using System;

namespace GenericDto.Core.Attributes;

/// <summary>
/// Marks a property to be excluded from DTO generation.
/// This is a shorthand alternative to [DtoProperty(Ignore = true)].
/// </summary>
/// <example>
/// <code>
/// public class Customer
/// {
///     public string Name { get; set; }
///     
///     [DtoIgnore]
///     public string InternalPassword { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class DtoIgnoreAttribute : Attribute
{
}
