using System;

namespace GenericDto.Core.Attributes;

/// <summary>
/// Customizes DTO property generation for a specific property.
/// Use this attribute to rename, change type, set default values, or apply validation.
/// </summary>
/// <example>
/// <code>
/// public class Customer
/// {
///     [DtoProperty(Name = "CustomerName", ForceNullable = false)]
///     public string? Name { get; set; }
///     
///     [DtoProperty(Ignore = true)]
///     public string InternalCode { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class DtoPropertyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a custom name for the DTO property.
    /// If null, the original property name is used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets whether to ignore this property in the DTO generation.
    /// When true, the property will not be included in the generated DTO.
    /// Default is false.
    /// </summary>
    public bool Ignore { get; set; }

    /// <summary>
    /// Gets or sets a custom type for the DTO property.
    /// Use this to change the property type in the generated DTO.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    /// Gets or sets the default value expression for the property.
    /// The value should be a valid C# expression as a string.
    /// Example: "string.Empty", "0", "new List&lt;int&gt;()"
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets whether to force the property to be nullable.
    /// When true, the property will be nullable in the DTO regardless of source.
    /// When false, the property will be non-nullable.
    /// When null (default), inherits nullability from the source property.
    /// </summary>
    public bool? ForceNullable { get; set; }

    /// <summary>
    /// Gets or sets the order of this property in the generated DTO.
    /// Lower values appear first. Default is 0.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets custom XML documentation for the property.
    /// If null, auto-generated documentation is used.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the maximum length validation for string properties.
    /// When set, adds [MaxLength] attribute to the generated property.
    /// </summary>
    public int MaxLength { get; set; } = -1;

    /// <summary>
    /// Gets or sets the minimum length validation for string properties.
    /// When set, adds [MinLength] attribute to the generated property.
    /// </summary>
    public int MinLength { get; set; } = -1;

    /// <summary>
    /// Gets or sets a regular expression pattern for string validation.
    /// When set, adds [RegularExpression] attribute to the generated property.
    /// </summary>
    public string? Pattern { get; set; }

    /// <summary>
    /// Gets or sets the minimum value for numeric properties.
    /// When set with MaxValue, adds [Range] attribute to the generated property.
    /// </summary>
    public double MinValue { get; set; } = double.MinValue;

    /// <summary>
    /// Gets or sets the maximum value for numeric properties.
    /// When set with MinValue, adds [Range] attribute to the generated property.
    /// </summary>
    public double MaxValue { get; set; } = double.MaxValue;
}
