using Microsoft.CodeAnalysis;

namespace GenericDto.Analyzers.Generators.Incremental;

internal readonly struct DtoGenerationContext
{
    public INamedTypeSymbol SourceType { get; }
    public AttributeData GenerateDtoAttribute { get; }
    public string TargetNamespace { get; }
    public string DtoClassName { get; }
    public List<PropertyContext> Properties { get; }

    public DtoGenerationContext(INamedTypeSymbol sourceType, AttributeData generateDtoAttribute, string targetNamespace, string dtoClassName, List<PropertyContext> properties)
    {
        SourceType = sourceType;
        GenerateDtoAttribute = generateDtoAttribute;
        TargetNamespace = targetNamespace;
        DtoClassName = dtoClassName;
        Properties = properties;
    }
}

internal readonly struct PropertyContext
{
    public string PropertyName { get; }
    public string PropertyType { get; }
    public bool IsNullable { get; }
    public bool HasDefaultValue { get; }
    public string? DefaultValue { get; }
    public bool IsRequired { get; }
    public bool IsReadOnly { get; }
    public IPropertySymbol SourceProperty { get; }

    public PropertyContext(string propertyName, string propertyType, bool isNullable, bool hasDefaultValue, string? defaultValue, bool isRequired, bool isReadOnly, IPropertySymbol sourceProperty)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
        IsNullable = isNullable;
        HasDefaultValue = hasDefaultValue;
        DefaultValue = defaultValue;
        IsRequired = isRequired;
        IsReadOnly = isReadOnly;
        SourceProperty = sourceProperty;
    }
}
