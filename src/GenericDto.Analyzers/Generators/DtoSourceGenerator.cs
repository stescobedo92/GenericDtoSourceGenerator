using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using GenericDto.Analyzers.Constants;
using GenericDto.Analyzers.Diagnostics;
using GenericDto.Analyzers.Extensions;
using GenericDto.Analyzers.Generators.Incremental;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GenericDto.Analyzers.Generators;

/// <summary>
/// Incremental source generator that generates DTO classes from types marked with [GenericDto] attribute.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DtoSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "GenericDtoAttributeMarker.g.cs",
            SourceText.From(GenerateAttributeMarker(), Encoding.UTF8)));

        var dtoContexts = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: GeneratorConstants.GenericDtoCoreAttributeFullName,
            predicate: static (node, _) => node is TypeDeclarationSyntax,
            transform: static (ctx, ct) => GetDtoGenerationContexts(ctx, ct));

        context.RegisterSourceOutput(dtoContexts.Collect(), static (spc, source) => Execute(source, spc));
    }

    private static ImmutableArray<DtoGenerationContext> GetDtoGenerationContexts(
        GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
            return ImmutableArray<DtoGenerationContext>.Empty;

        var builder = ImmutableArray.CreateBuilder<DtoGenerationContext>();
        foreach (var attribute in context.Attributes.Where(IsGenericDtoAttribute))
        {
            builder.Add(CreateDtoGenerationContext(typeSymbol, attribute, cancellationToken));
        }

        return builder.ToImmutable();
    }

    private static bool IsGenericDtoAttribute(AttributeData attribute)
    {
        return attribute.AttributeClass?.ToDisplayString() == GeneratorConstants.GenericDtoCoreAttributeFullName;
    }

    private static DtoGenerationContext CreateDtoGenerationContext(
        INamedTypeSymbol typeSymbol,
        AttributeData attribute,
        CancellationToken cancellationToken)
    {
        var dtoName = attribute.GetNamedArgument<string>("DtoName");
        var targetNamespace = attribute.GetNamedArgument<string>("Namespace");
        var useRecord = attribute.GetNamedArgument<bool?>("UseRecord") ?? false;
        var accessModifier = attribute.GetNamedArgument<string>("AccessModifier") ?? "public";
        var generateParameterlessConstructor = attribute.GetNamedArgument<bool?>("GenerateParameterlessConstructor") ?? true;
        var implementIEquatable = attribute.GetNamedArgument<bool?>("ImplementIEquatable") ?? false;
        var implementIValidatableObject = attribute.GetNamedArgument<bool?>("ImplementIValidatableObject") ?? false;
        var includeInheritedProperties = attribute.GetNamedArgument<bool?>("IncludeInheritedProperties") ?? true;
        var generateDocumentation = attribute.GetNamedArgument<bool?>("GenerateDocumentation") ?? true;
        var generateMappers = attribute.GetNamedArgument<bool?>("GenerateMappers") ?? true;
        var generateToDto = attribute.GetNamedArgument<bool?>("GenerateToDto") ?? true;
        var generateToEntity = attribute.GetNamedArgument<bool?>("GenerateToEntity") ?? true;
        var generateUpdateFrom = attribute.GetNamedArgument<bool?>("GenerateUpdateFrom") ?? true;
        var generateCollectionMappers = attribute.GetNamedArgument<bool?>("GenerateCollectionMappers") ?? true;
        var generateToString = attribute.GetNamedArgument<bool?>("GenerateToString") ?? true;
        var useInitOnlyProperties = attribute.GetNamedArgument<bool?>("UseInitOnlyProperties") ?? false;
        var useRequiredMembers = attribute.GetNamedArgument<bool?>("UseRequiredMembers") ?? false;
        var generateJsonAttributes = attribute.GetNamedArgument<bool?>("GenerateJsonAttributes") ?? true;

        var finalDtoName = string.IsNullOrWhiteSpace(dtoName)
            ? typeSymbol.Name + GeneratorConstants.DtoSuffix
            : dtoName!;

        var sourceNamespace = typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : typeSymbol.ContainingNamespace.ToDisplayString();

        var finalNamespace = string.IsNullOrWhiteSpace(targetNamespace)
            ? string.IsNullOrWhiteSpace(sourceNamespace) ? "Dto" : sourceNamespace + GeneratorConstants.DtoNamespaceSuffix
            : targetNamespace!;

        var mapperNamespace = attribute.GetNamedArgument<string>("MapperNamespace");
        var finalMapperNamespace = string.IsNullOrWhiteSpace(mapperNamespace) ? finalNamespace : mapperNamespace!;

        var mapperClassName = attribute.GetNamedArgument<string>("MapperClassName");
        var finalMapperClassName = string.IsNullOrWhiteSpace(mapperClassName)
            ? finalDtoName + "MapperExtensions"
            : mapperClassName!;

        var properties = CollectProperties(typeSymbol, includeInheritedProperties, cancellationToken);

        return new DtoGenerationContext(
            typeSymbol,
            attribute,
            finalNamespace,
            finalDtoName,
            finalMapperNamespace,
            finalMapperClassName,
            properties,
            useRecord,
            accessModifier,
            generateParameterlessConstructor,
            implementIEquatable,
            implementIValidatableObject,
            includeInheritedProperties,
            generateDocumentation,
            generateMappers,
            generateToDto,
            generateToEntity,
            generateUpdateFrom,
            generateCollectionMappers,
            generateToString,
            useInitOnlyProperties,
            useRequiredMembers,
            generateJsonAttributes);
    }

    private static string GenerateAttributeMarker()
    {
        return @"// <auto-generated/>
// This file was generated by GenericDto Source Generator

[assembly: GenericDto.Analyzers.GenericDtoGeneratedAttribute]

namespace GenericDto.Analyzers
{
    [global::System.AttributeUsage(global::System.AttributeTargets.Assembly)]
    internal sealed class GenericDtoGeneratedAttribute : global::System.Attribute { }
}
";
    }

    private static ImmutableArray<PropertyContext> CollectProperties(
        INamedTypeSymbol typeSymbol,
        bool includeInheritedProperties,
        CancellationToken cancellationToken)
    {
        var properties = new List<(PropertyContext Property, int Index)>();
        var processedNames = new HashSet<string>(StringComparer.Ordinal);
        var currentType = typeSymbol;
        var index = 0;

        while (currentType != null && currentType.SpecialType != SpecialType.System_Object)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var member in currentType.GetMembers())
            {
                if (member is not IPropertySymbol propertySymbol)
                    continue;

                if (!processedNames.Add(propertySymbol.Name))
                    continue;

                if (ShouldIgnoreProperty(propertySymbol))
                    continue;

                if (propertySymbol.DeclaredAccessibility != Accessibility.Public ||
                    propertySymbol.IsStatic ||
                    propertySymbol.IsIndexer ||
                    propertySymbol.GetMethod is null)
                    continue;

                properties.Add((CreatePropertyContext(propertySymbol), index++));
            }

            if (!includeInheritedProperties)
                break;

            currentType = currentType.BaseType;
        }

        return properties
            .OrderBy(p => p.Property.Order)
            .ThenBy(p => p.Index)
            .Select(p => p.Property)
            .ToImmutableArray();
    }

    private static bool ShouldIgnoreProperty(IPropertySymbol property)
    {
        if (property.HasAttribute(GeneratorConstants.DtoIgnoreAttributeFullName))
            return true;

        var dtoPropertyAttr = property.GetAttribute(GeneratorConstants.DtoPropertyAttributeFullName);
        return dtoPropertyAttr?.GetNamedArgument<bool?>("Ignore") == true;
    }

    private static PropertyContext CreatePropertyContext(IPropertySymbol property)
    {
        var dtoPropertyAttr = property.GetAttribute(GeneratorConstants.DtoPropertyAttributeFullName);
        var customName = dtoPropertyAttr?.GetNamedArgument<string>("Name");
        var propertyName = string.IsNullOrWhiteSpace(customName) ? property.Name : customName!;
        var effectiveType = GetEffectivePropertyType(property, dtoPropertyAttr);
        var propertyType = effectiveType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var forceNullableValue = dtoPropertyAttr?.GetNamedArgument<int?>("ForceNullable");
        bool isNullable;
        if (forceNullableValue == 1)
            isNullable = true;
        else if (forceNullableValue == 2)
            isNullable = false;
        else
            isNullable = effectiveType.IsNullable();

        var defaultValue = dtoPropertyAttr?.GetNamedArgument<string>("DefaultValue");
        var hasDefaultValue = !string.IsNullOrWhiteSpace(defaultValue);
        var validations = GetValidationContexts(property);
        var isRequired = (!isNullable && !hasDefaultValue && !effectiveType.IsValueType) ||
                         validations.Any(v => v.Required);
        var description = dtoPropertyAttr?.GetNamedArgument<string>("Description");
        var maxLength = dtoPropertyAttr?.GetNamedArgument<int?>("MaxLength") ?? -1;
        var minLength = dtoPropertyAttr?.GetNamedArgument<int?>("MinLength") ?? -1;
        var pattern = dtoPropertyAttr?.GetNamedArgument<string>("Pattern");
        var minValue = dtoPropertyAttr?.GetNamedArgument<double?>("MinValue") ?? double.MinValue;
        var maxValue = dtoPropertyAttr?.GetNamedArgument<double?>("MaxValue") ?? double.MaxValue;
        var order = dtoPropertyAttr?.GetNamedArgument<int?>("Order") ?? 0;

        return new PropertyContext(
            propertyName,
            propertyType,
            effectiveType,
            isNullable,
            hasDefaultValue,
            defaultValue,
            isRequired,
            property.SetMethod is null,
            property,
            description,
            maxLength,
            minLength,
            pattern,
            minValue,
            maxValue,
            order,
            validations,
            dtoPropertyAttr?.GetNamedArgument<string>("MapFrom"),
            dtoPropertyAttr?.GetNamedArgument<string>("MapTo"),
            GetNamedTypeArgument(dtoPropertyAttr, "ConverterType"),
            dtoPropertyAttr?.GetNamedArgument<string>("ConverterMethod"),
            dtoPropertyAttr?.GetNamedArgument<bool?>("IgnoreReverseMap") ?? false,
            dtoPropertyAttr?.GetNamedArgument<bool?>("Flatten") ?? false,
            dtoPropertyAttr?.GetNamedArgument<bool?>("Sensitive") ?? false,
            dtoPropertyAttr?.GetNamedArgument<bool?>("JsonIgnore") ?? false,
            dtoPropertyAttr?.GetNamedArgument<string>("JsonPropertyName"),
            GetNamedTypeArgument(dtoPropertyAttr, "JsonConverterType"));
    }

    private static ImmutableArray<ValidationContext> GetValidationContexts(IPropertySymbol property)
    {
        return property.GetAttributes(GeneratorConstants.DtoValidationAttributeFullName)
            .Select(attribute => new ValidationContext(
                attribute.GetNamedArgument<bool?>("Required") ?? false,
                attribute.GetNamedArgument<bool?>("EmailAddress") ?? false,
                attribute.GetNamedArgument<bool?>("Phone") ?? false,
                attribute.GetNamedArgument<bool?>("Url") ?? false,
                attribute.GetNamedArgument<bool?>("CreditCard") ?? false,
                attribute.GetNamedArgument<string>("ErrorMessage"),
                attribute.GetNamedArgument<string>("CompareProperty"),
                GetNamedTypeArgument(attribute, "CustomValidationType"),
                attribute.GetNamedArgument<string>("CustomValidationMethod")))
            .ToImmutableArray();
    }

    private static string? GetNamedTypeArgument(AttributeData? attribute, string argumentName)
    {
        if (attribute is null)
            return null;

        var value = attribute.NamedArguments
            .FirstOrDefault(na => na.Key == argumentName)
            .Value;

        if (!value.IsNull &&
            value.Kind == TypedConstantKind.Type &&
            value.Value is ITypeSymbol typeSymbol)
        {
            return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        return null;
    }

    private static ITypeSymbol GetEffectivePropertyType(IPropertySymbol property, AttributeData? dtoPropertyAttr)
    {
        var customTypeName = GetNamedTypeArgument(dtoPropertyAttr, "Type");
        if (customTypeName is null || dtoPropertyAttr is null)
            return property.Type;

        var customType = dtoPropertyAttr.NamedArguments
            .FirstOrDefault(na => na.Key == "Type")
            .Value;

        return customType.Value is ITypeSymbol customTypeSymbol ? customTypeSymbol : property.Type;
    }

    private static void Execute(ImmutableArray<ImmutableArray<DtoGenerationContext>> groupedTypes, SourceProductionContext context)
    {
        if (groupedTypes.IsDefaultOrEmpty)
            return;

        var generatedDtos = new Dictionary<string, DtoGenerationContext>(StringComparer.Ordinal);
        foreach (var dtoContext in groupedTypes.SelectMany(g => g))
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var fullName = $"{dtoContext.TargetNamespace}.{dtoContext.DtoClassName}";
            if (generatedDtos.ContainsKey(fullName))
            {
                Report(context, DiagnosticDescriptors.DuplicateDtoName, dtoContext.SourceType.Locations.FirstOrDefault(), dtoContext.DtoClassName);
                continue;
            }

            generatedDtos[fullName] = dtoContext;
            ValidateConfiguration(dtoContext, context);
            ValidatePropertyValidationAttributes(dtoContext, context);

            var code = DtoCodeBuilder.GenerateDto(dtoContext);
            context.AddSource($"{SanitizeHintName(fullName)}.g.cs", SourceText.From(code, Encoding.UTF8));

            if (dtoContext.GenerateMappers)
            {
                var mapperCode = MapperCodeBuilder.GenerateMapper(dtoContext);
                context.AddSource($"{SanitizeHintName(fullName)}Mapper.g.cs", SourceText.From(mapperCode, Encoding.UTF8));
            }
        }
    }

    private static void ValidateConfiguration(DtoGenerationContext dtoContext, SourceProductionContext context)
    {
        if (dtoContext.AccessModifier is not ("public" or "internal"))
        {
            Report(context, DiagnosticDescriptors.InvalidDtoConfiguration, dtoContext.SourceType.Locations.FirstOrDefault(),
                "AccessModifier must be either \"public\" or \"internal\" for generated namespace-level DTOs.");
        }

        if (!SyntaxFacts.IsValidIdentifier(dtoContext.DtoClassName))
        {
            Report(context, DiagnosticDescriptors.InvalidDtoConfiguration, dtoContext.SourceType.Locations.FirstOrDefault(),
                $"DtoName '{dtoContext.DtoClassName}' is not a valid C# identifier.");
        }

        if (!SyntaxFacts.IsValidIdentifier(dtoContext.MapperClassName))
        {
            Report(context, DiagnosticDescriptors.InvalidDtoConfiguration, dtoContext.SourceType.Locations.FirstOrDefault(),
                $"MapperClassName '{dtoContext.MapperClassName}' is not a valid C# identifier.");
        }

        if (!IsValidNamespace(dtoContext.TargetNamespace))
        {
            Report(context, DiagnosticDescriptors.InvalidDtoConfiguration, dtoContext.SourceType.Locations.FirstOrDefault(),
                $"Namespace '{dtoContext.TargetNamespace}' is not a valid C# namespace.");
        }

        if (!IsValidNamespace(dtoContext.MapperNamespace))
        {
            Report(context, DiagnosticDescriptors.InvalidDtoConfiguration, dtoContext.SourceType.Locations.FirstOrDefault(),
                $"MapperNamespace '{dtoContext.MapperNamespace}' is not a valid C# namespace.");
        }
    }

    private static bool IsValidNamespace(string value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Split('.').All(part => SyntaxFacts.IsValidIdentifier(part));
    }

    private static void ValidatePropertyValidationAttributes(DtoGenerationContext dtoContext, SourceProductionContext context)
    {
        foreach (var property in dtoContext.Properties)
        {
            var propertyType = property.EffectiveType;
            var propertyName = property.PropertyName;
            var propertyTypeDisplay = propertyType.ToDisplayString();

            var hasStringValidation = property.MaxLength > 0 ||
                                      property.MinLength > 0 ||
                                      !string.IsNullOrWhiteSpace(property.Pattern) ||
                                      property.Validations.Any(v => v.EmailAddress || v.Phone || v.Url || v.CreditCard);
            if (hasStringValidation && !propertyType.IsStringType())
            {
                Report(context, DiagnosticDescriptors.StringValidationOnNonStringProperty,
                    property.SourceProperty.Locations.FirstOrDefault(), propertyName, propertyTypeDisplay);
            }

            var hasNumericValidation = property.MinValue != double.MinValue || property.MaxValue != double.MaxValue;
            if (hasNumericValidation && !propertyType.IsNumericType())
            {
                Report(context, DiagnosticDescriptors.NumericValidationOnNonNumericProperty,
                    property.SourceProperty.Locations.FirstOrDefault(), propertyName, propertyTypeDisplay);
            }

            if (property.MinLength > 0 && property.MaxLength > 0 && property.MinLength > property.MaxLength)
            {
                Report(context, DiagnosticDescriptors.InvalidStringValidationRange,
                    property.SourceProperty.Locations.FirstOrDefault(), propertyName, property.MinLength, property.MaxLength);
            }

            if (property.MinValue != double.MinValue && property.MaxValue != double.MaxValue && property.MinValue > property.MaxValue)
            {
                Report(context, DiagnosticDescriptors.InvalidNumericValidationRange,
                    property.SourceProperty.Locations.FirstOrDefault(), propertyName, property.MinValue, property.MaxValue);
            }
        }
    }

    private static void Report(SourceProductionContext context, DiagnosticDescriptor descriptor, Location? location, params object[] args)
    {
        context.ReportDiagnostic(Diagnostic.Create(descriptor, location, args));
    }

    private static string SanitizeHintName(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            builder.Append(char.IsLetterOrDigit(ch) || ch == '.' || ch == '_' ? ch : '_');
        }

        return builder.ToString();
    }
}
