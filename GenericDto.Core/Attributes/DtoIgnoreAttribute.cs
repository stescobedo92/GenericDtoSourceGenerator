namespace GenericDto.Core.Attributes;

/// <summary>
/// Marks a property to be excluded from DTO generation
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class DtoIgnoreAttribute : Attribute
{
}
