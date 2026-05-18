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
///     [DtoProperty(Name = "CustomerName", ForceNullable = NullableOption.True)]
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
    /// Use NullableOption.Inherit (default) to inherit from source property.
    /// Use NullableOption.True to force nullable.
    /// Use NullableOption.False to force non-nullable.
    /// </summary>
    public NullableOption ForceNullable { get; set; } = NullableOption.Inherit;

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

    /// <summary>
    /// Gets or sets an alternate source property name to read when mapping from entity to DTO.
    /// </summary>
    public string? MapFrom { get; set; }

    /// <summary>
    /// Gets or sets an alternate destination property name to write when mapping from DTO to entity.
    /// </summary>
    public string? MapTo { get; set; }

    /// <summary>
    /// Gets or sets a converter type used for custom mapping.
    /// The converter method is resolved from ConverterMethod.
    /// </summary>
    public Type? ConverterType { get; set; }

    /// <summary>
    /// Gets or sets the converter method name used for custom mapping.
    /// The method should be static and accept the source value.
    /// </summary>
    public string? ConverterMethod { get; set; }

    /// <summary>
    /// Gets or sets whether this property should be omitted from reverse mapping.
    /// </summary>
    public bool IgnoreReverseMap { get; set; }

    /// <summary>
    /// Gets or sets whether this property uses a flattened source path.
    /// Combine with MapFrom, for example MapFrom = "Address.City".
    /// </summary>
    public bool Flatten { get; set; }

    /// <summary>
    /// Gets or sets whether this property should be excluded from generated ToString output.
    /// </summary>
    public bool Sensitive { get; set; }

    /// <summary>
    /// Gets or sets whether to emit System.Text.Json.Serialization.JsonIgnoreAttribute.
    /// </summary>
    public bool JsonIgnore { get; set; }

    /// <summary>
    /// Gets or sets the JSON property name emitted with JsonPropertyNameAttribute.
    /// </summary>
    public string? JsonPropertyName { get; set; }

    /// <summary>
    /// Gets or sets the JSON converter type emitted with JsonConverterAttribute.
    /// </summary>
    public Type? JsonConverterType { get; set; }
}

/// <summary>
/// Specifies nullable behavior for DTO property generation.
/// </summary>
public enum NullableOption
{
    /// <summary>
    /// Inherit nullability from the source property.
    /// </summary>
    Inherit = 0,

    /// <summary>
    /// Force the property to be nullable.
    /// </summary>
    True = 1,

    /// <summary>
    /// Force the property to be non-nullable.
    /// </summary>
    False = 2
}
