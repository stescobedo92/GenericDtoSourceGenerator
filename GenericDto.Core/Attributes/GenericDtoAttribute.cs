namespace GenericDto.Core.Attributes;

/// <summary>
/// Marks a class or record for DTO generation
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class GenericDtoAttribute : Attribute
{
    /// <summary>
    /// The name of the generated DTO class. If null, uses {SourceName}Dto
    /// </summary>
    public string? DtoName { get; set; }

    /// <summary>
    /// Namespace for the generated DTO. If null, uses {SourceNamespace}.Dto
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// Whether to generate a record instead of a class
    /// </summary>
    public bool UseRecord { get; set; }

    /// <summary>
    /// Access modifier for the generated DTO
    /// </summary>
    public string AccessModifier { get; set; } = "public";

    /// <summary>
    /// Whether to generate a parameterless constructor
    /// </summary>
    public bool GenerateParameterlessConstructor { get; set; } = true;

    /// <summary>
    /// Whether to implement IEquatable<T>
    /// </summary>
    public bool ImplementIEquatable { get; set; }

    /// <summary>
    /// Whether to generate IValidatableObject implementation
    /// </summary>
    public bool ImplementIValidatableObject { get; set; }

    /// <summary>
    /// Additional using statements for the generated DTO
    /// </summary>
    public string[] AdditionalUsings { get; set; } = Array.Empty<string>();
}
