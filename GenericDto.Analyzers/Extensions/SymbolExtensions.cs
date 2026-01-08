using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GenericDto.Analyzers.Extensions;

/// <summary>
/// Extension methods for working with ISymbol and related types.
/// </summary>
internal static class SymbolExtensions
{
    /// <summary>
    /// Checks if the symbol has an attribute with the specified full name.
    /// </summary>
    public static bool HasAttribute(this ISymbol symbol, string attributeFullName)
    {
        return symbol.GetAttributes().Any(attr =>
            attr.AttributeClass?.ToDisplayString() == attributeFullName);
    }

    /// <summary>
    /// Gets the first attribute with the specified full name, or null if not found.
    /// </summary>
    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeFullName)
    {
        return symbol.GetAttributes().FirstOrDefault(attr =>
            attr.AttributeClass?.ToDisplayString() == attributeFullName);
    }

    /// <summary>
    /// Gets all attributes with the specified full name.
    /// </summary>
    public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, string attributeFullName)
    {
        return symbol.GetAttributes().Where(attr =>
            attr.AttributeClass?.ToDisplayString() == attributeFullName);
    }

    /// <summary>
    /// Checks if the type is a reference type (excluding string).
    /// </summary>
    public static bool IsReferenceType(this ITypeSymbol type)
    {
        return type.IsReferenceType && type.SpecialType != SpecialType.System_String;
    }

    /// <summary>
    /// Checks if the type is a value type.
    /// </summary>
    public static bool IsValueType(this ITypeSymbol type)
    {
        return type.IsValueType;
    }

    /// <summary>
    /// Checks if the type is nullable (either nullable reference type or Nullable&lt;T&gt;).
    /// </summary>
    public static bool IsNullable(this ITypeSymbol type)
    {
        // Check nullable reference type annotation
        if (type.NullableAnnotation == NullableAnnotation.Annotated)
            return true;

        // Check Nullable<T>
        if (type is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if the named type is a record.
    /// </summary>
    public static bool IsRecord(this INamedTypeSymbol type)
    {
        return type.IsRecord;
    }

    /// <summary>
    /// Gets the effective accessibility of a symbol.
    /// </summary>
    public static Accessibility GetEffectiveAccessibility(this ISymbol symbol)
    {
        return symbol.DeclaredAccessibility;
    }

    /// <summary>
    /// Checks if the symbol is accessible from a specific type.
    /// </summary>
    public static bool IsAccessibleFrom(this ISymbol symbol, ITypeSymbol fromType)
    {
        return symbol.DeclaredAccessibility is
            Accessibility.Public or
            Accessibility.Internal or
            Accessibility.ProtectedOrInternal;
    }

    /// <summary>
    /// Gets the fully qualified name of a type symbol.
    /// </summary>
    public static string GetFullQualifiedName(this ITypeSymbol type, bool includeNullable = false)
    {
        var format = SymbolDisplayFormat.FullyQualifiedFormat
            .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters);

        if (includeNullable)
        {
            format = format.WithMiscellaneousOptions(
                SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
        }

        var displayString = type.ToDisplayString(format);
        return includeNullable ? displayString : displayString.TrimEnd('?');
    }

    /// <summary>
    /// Gets a named argument value from an attribute.
    /// </summary>
    public static T? GetNamedArgument<T>(this AttributeData attribute, string argumentName)
    {
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == argumentName)
            {
                var value = namedArg.Value;

                // Handle null values
                if (value.IsNull)
                    return default;

                // Handle arrays - only access Values if Kind is Array
                if (value.Kind == TypedConstantKind.Array)
                {
                    if (typeof(T) == typeof(string[]))
                    {
                        var stringArray = value.Values
                            .Where(v => !v.IsNull && v.Value is string)
                            .Select(v => (string)v.Value!)
                            .ToArray();
                        return (T)(object)stringArray;
                    }
                    // Return default for other array types we don't handle
                    return default;
                }

                // Handle direct value
                if (value.Value is T typedValue)
                    return typedValue;

                // Handle enum conversion
                if (typeof(T).IsEnum && value.Value != null)
                {
                    return (T)Enum.ToObject(typeof(T), value.Value);
                }

                // Handle nullable types
                var underlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (underlyingType != null && value.Value != null)
                {
                    if (underlyingType.IsEnum)
                    {
                        return (T)Enum.ToObject(underlyingType, value.Value);
                    }
                    if (underlyingType.IsAssignableFrom(value.Value.GetType()))
                    {
                        return (T)value.Value;
                    }
                }

                break;
            }
        }
        return default;
    }

    /// <summary>
    /// Gets a named argument as an array of strings.
    /// </summary>
    public static string[] GetNamedArgumentAsStringArray(this AttributeData attribute, string argumentName)
    {
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == argumentName)
            {
                var value = namedArg.Value;

                // Check if it's null
                if (value.IsNull)
                    return Array.Empty<string>();

                // Check if it's an array type (Kind == Array)
                if (value.Kind == TypedConstantKind.Array && !value.Values.IsDefaultOrEmpty)
                {
                    return value.Values
                        .Where(v => !v.IsNull && v.Value is string)
                        .Select(v => (string)v.Value!)
                        .ToArray();
                }

                // If it's a single string value, return it as array
                if (value.Value is string singleValue)
                {
                    return new[] { singleValue };
                }
            }
        }
        return Array.Empty<string>();
    }

    /// <summary>
    /// Gets all members from a type and its base types.
    /// </summary>
    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
    {
        var current = type;
        while (current != null && current.SpecialType != SpecialType.System_Object)
        {
            foreach (var member in current.GetMembers())
            {
                yield return member;
            }
            current = current.BaseType;
        }
    }

    /// <summary>
    /// Gets all public properties from a type and its base types.
    /// </summary>
    public static IEnumerable<IPropertySymbol> GetAllPublicProperties(this ITypeSymbol type)
    {
        return type.GetAllMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public &&
                       !p.IsStatic &&
                       !p.IsIndexer);
    }

    /// <summary>
    /// Checks if the type has a public parameterless constructor.
    /// </summary>
    public static bool HasPublicParameterlessConstructor(this INamedTypeSymbol type)
    {
        return type.InstanceConstructors.Any(c =>
            c.Parameters.Length == 0 &&
            c.DeclaredAccessibility == Accessibility.Public);
    }

    /// <summary>
    /// Gets the namespace as a string, or empty string for global namespace.
    /// </summary>
    public static string GetNamespaceString(this ISymbol symbol)
    {
        var ns = symbol.ContainingNamespace;
        return ns.IsGlobalNamespace ? string.Empty : ns.ToDisplayString();
    }

    /// <summary>
    /// Checks if the property has both a getter and setter.
    /// </summary>
    public static bool HasGetterAndSetter(this IPropertySymbol property)
    {
        return property.GetMethod != null && property.SetMethod != null;
    }

    /// <summary>
    /// Checks if the property setter is init-only.
    /// </summary>
    public static bool IsInitOnly(this IPropertySymbol property)
    {
        return property.SetMethod?.IsInitOnly ?? false;
    }
}
