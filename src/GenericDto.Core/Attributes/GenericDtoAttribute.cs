using System;

namespace GenericDto.Core.Attributes;

/// <summary>
/// Marks a class, struct, or record for DTO generation.
/// Apply this attribute to generate a corresponding Data Transfer Object automatically.
/// </summary>
/// <example>
/// <code>
/// [GenericDto(DtoName = "CustomerResponse", UseRecord = true)]
/// public class Customer
/// {
///     public int Id { get; set; }
///     public string Name { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class GenericDtoAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the generated DTO class.
    /// If null or empty, defaults to {SourceTypeName}Dto.
    /// </summary>
    public string? DtoName { get; set; }

    /// <summary>
    /// Gets or sets the namespace for the generated DTO.
    /// If null or empty, defaults to {SourceNamespace}.Dto.
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// Gets or sets whether to generate a record instead of a class.
    /// Records provide value-based equality and immutability by default.
    /// Default is false.
    /// </summary>
    public bool UseRecord { get; set; }

    /// <summary>
    /// Gets or sets the access modifier for the generated DTO.
    /// Valid values: "public", "internal", "private".
    /// Default is "public".
    /// </summary>
    public string AccessModifier { get; set; } = "public";

    /// <summary>
    /// Gets or sets whether to generate a parameterless constructor.
    /// Useful for serialization/deserialization scenarios.
    /// Default is true.
    /// </summary>
    public bool GenerateParameterlessConstructor { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to implement IEquatable&lt;T&gt;.
    /// Provides type-safe equality comparison.
    /// Not needed when UseRecord is true (records implement this by default).
    /// Default is false.
    /// </summary>
    public bool ImplementIEquatable { get; set; }

    /// <summary>
    /// Gets or sets whether to generate IValidatableObject implementation.
    /// Enables custom validation logic for the DTO.
    /// Default is false.
    /// </summary>
    public bool ImplementIValidatableObject { get; set; }

    /// <summary>
    /// Gets or sets additional using statements for the generated DTO.
    /// Useful when custom types are referenced in property types.
    /// </summary>
    public string[] AdditionalUsings { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets whether to include inherited properties in the DTO.
    /// Default is true.
    /// </summary>
    public bool IncludeInheritedProperties { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to generate XML documentation comments.
    /// Default is true.
    /// </summary>
    public bool GenerateDocumentation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to generate mapping extension methods.
    /// Default is true.
    /// </summary>
    public bool GenerateMappers { get; set; } = true;
}
