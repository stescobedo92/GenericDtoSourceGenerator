namespace GenericDto.Core.Attributes;

/// <summary>
/// Customize DTO property generation for a specific property
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class DtoPropertyAttribute : Attribute
{
    /// <summary>
    /// Custom name for the DTO property
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Whether to ignore this property in the DTO
    /// </summary>
    public bool Ignore { get; set; }

    /// <summary>
    /// Custom type for the DTO property
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    /// Default value for the property
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Whether to make the property nullable
    /// </summary>
    public bool? ForceNullable { get; set; }
}
