using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace GenericDto.Analyzers.Generators.Incremental;

/// <summary>
/// Contains the context information needed to generate a DTO.
/// </summary>
internal readonly struct DtoGenerationContext
{
    public INamedTypeSymbol SourceType { get; }
    public AttributeData GenerateDtoAttribute { get; }
    public string TargetNamespace { get; }
    public string DtoClassName { get; }
    public string MapperNamespace { get; }
    public string MapperClassName { get; }
    public ImmutableArray<PropertyContext> Properties { get; }
    public bool UseRecord { get; }
    public string AccessModifier { get; }
    public bool GenerateParameterlessConstructor { get; }
    public bool ImplementIEquatable { get; }
    public bool ImplementIValidatableObject { get; }
    public bool IncludeInheritedProperties { get; }
    public bool GenerateDocumentation { get; }
    public bool GenerateMappers { get; }
    public bool GenerateToDto { get; }
    public bool GenerateToEntity { get; }
    public bool GenerateUpdateFrom { get; }
    public bool GenerateCollectionMappers { get; }
    public bool GenerateToString { get; }
    public bool UseInitOnlyProperties { get; }
    public bool UseRequiredMembers { get; }
    public bool GenerateJsonAttributes { get; }

    public DtoGenerationContext(
        INamedTypeSymbol sourceType,
        AttributeData generateDtoAttribute,
        string targetNamespace,
        string dtoClassName,
        string mapperNamespace,
        string mapperClassName,
        ImmutableArray<PropertyContext> properties,
        bool useRecord,
        string accessModifier,
        bool generateParameterlessConstructor,
        bool implementIEquatable,
        bool implementIValidatableObject,
        bool includeInheritedProperties,
        bool generateDocumentation,
        bool generateMappers,
        bool generateToDto,
        bool generateToEntity,
        bool generateUpdateFrom,
        bool generateCollectionMappers,
        bool generateToString,
        bool useInitOnlyProperties,
        bool useRequiredMembers,
        bool generateJsonAttributes)
    {
        SourceType = sourceType;
        GenerateDtoAttribute = generateDtoAttribute;
        TargetNamespace = targetNamespace;
        DtoClassName = dtoClassName;
        MapperNamespace = mapperNamespace;
        MapperClassName = mapperClassName;
        Properties = properties;
        UseRecord = useRecord;
        AccessModifier = accessModifier;
        GenerateParameterlessConstructor = generateParameterlessConstructor;
        ImplementIEquatable = implementIEquatable;
        ImplementIValidatableObject = implementIValidatableObject;
        IncludeInheritedProperties = includeInheritedProperties;
        GenerateDocumentation = generateDocumentation;
        GenerateMappers = generateMappers;
        GenerateToDto = generateToDto;
        GenerateToEntity = generateToEntity;
        GenerateUpdateFrom = generateUpdateFrom;
        GenerateCollectionMappers = generateCollectionMappers;
        GenerateToString = generateToString;
        UseInitOnlyProperties = useInitOnlyProperties;
        UseRequiredMembers = useRequiredMembers;
        GenerateJsonAttributes = generateJsonAttributes;
    }
}

/// <summary>
/// Contains the context information for a single property in the DTO.
/// </summary>
internal readonly struct PropertyContext
{
    public string PropertyName { get; }
    public string PropertyType { get; }
    public ITypeSymbol EffectiveType { get; }
    public bool IsNullable { get; }
    public bool HasDefaultValue { get; }
    public string? DefaultValue { get; }
    public bool IsRequired { get; }
    public bool IsReadOnly { get; }
    public IPropertySymbol SourceProperty { get; }
    public string? Description { get; }
    public int MaxLength { get; }
    public int MinLength { get; }
    public string? Pattern { get; }
    public double MinValue { get; }
    public double MaxValue { get; }
    public int Order { get; }
    public ImmutableArray<ValidationContext> Validations { get; }
    public string? MapFrom { get; }
    public string? MapTo { get; }
    public string? ConverterTypeName { get; }
    public string? ConverterMethod { get; }
    public bool IgnoreReverseMap { get; }
    public bool Flatten { get; }
    public bool Sensitive { get; }
    public bool JsonIgnore { get; }
    public string? JsonPropertyName { get; }
    public string? JsonConverterTypeName { get; }

    public PropertyContext(
        string propertyName,
        string propertyType,
        ITypeSymbol effectiveType,
        bool isNullable,
        bool hasDefaultValue,
        string? defaultValue,
        bool isRequired,
        bool isReadOnly,
        IPropertySymbol sourceProperty,
        string? description = null,
        int maxLength = -1,
        int minLength = -1,
        string? pattern = null,
        double minValue = double.MinValue,
        double maxValue = double.MaxValue,
        int order = 0,
        ImmutableArray<ValidationContext> validations = default,
        string? mapFrom = null,
        string? mapTo = null,
        string? converterTypeName = null,
        string? converterMethod = null,
        bool ignoreReverseMap = false,
        bool flatten = false,
        bool sensitive = false,
        bool jsonIgnore = false,
        string? jsonPropertyName = null,
        string? jsonConverterTypeName = null)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
        EffectiveType = effectiveType;
        IsNullable = isNullable;
        HasDefaultValue = hasDefaultValue;
        DefaultValue = defaultValue;
        IsRequired = isRequired;
        IsReadOnly = isReadOnly;
        SourceProperty = sourceProperty;
        Description = description;
        MaxLength = maxLength;
        MinLength = minLength;
        Pattern = pattern;
        MinValue = minValue;
        MaxValue = maxValue;
        Order = order;
        Validations = validations.IsDefault ? ImmutableArray<ValidationContext>.Empty : validations;
        MapFrom = mapFrom;
        MapTo = mapTo;
        ConverterTypeName = converterTypeName;
        ConverterMethod = converterMethod;
        IgnoreReverseMap = ignoreReverseMap;
        Flatten = flatten;
        Sensitive = sensitive;
        JsonIgnore = jsonIgnore;
        JsonPropertyName = jsonPropertyName;
        JsonConverterTypeName = jsonConverterTypeName;
    }
}

/// <summary>
/// Contains validation metadata for a generated DTO property.
/// </summary>
internal readonly struct ValidationContext
{
    public bool Required { get; }
    public bool EmailAddress { get; }
    public bool Phone { get; }
    public bool Url { get; }
    public bool CreditCard { get; }
    public string? ErrorMessage { get; }
    public string? CompareProperty { get; }
    public string? CustomValidationTypeName { get; }
    public string CustomValidationMethod { get; }

    public ValidationContext(
        bool required,
        bool emailAddress,
        bool phone,
        bool url,
        bool creditCard,
        string? errorMessage,
        string? compareProperty,
        string? customValidationTypeName,
        string? customValidationMethod)
    {
        Required = required;
        EmailAddress = emailAddress;
        Phone = phone;
        Url = url;
        CreditCard = creditCard;
        ErrorMessage = errorMessage;
        CompareProperty = compareProperty;
        CustomValidationTypeName = customValidationTypeName;
        CustomValidationMethod = string.IsNullOrWhiteSpace(customValidationMethod) ? "IsValid" : customValidationMethod!;
    }
}
