using System;

namespace GenericDto.Core.Attributes;

/// <summary>
/// Adds validation rules to a DTO property.
/// Multiple validation attributes can be applied to the same property.
/// </summary>
/// <example>
/// <code>
/// public class Customer
/// {
///     [DtoValidation(Required = true, ErrorMessage = "Email is required")]
///     [DtoValidation(EmailAddress = true)]
///     public string Email { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class DtoValidationAttribute : Attribute
{
    /// <summary>
    /// Gets or sets whether the property is required.
    /// Adds [Required] attribute to the generated property.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Gets or sets whether to validate the property as an email address.
    /// Adds [EmailAddress] attribute to the generated property.
    /// </summary>
    public bool EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets whether to validate the property as a phone number.
    /// Adds [Phone] attribute to the generated property.
    /// </summary>
    public bool Phone { get; set; }

    /// <summary>
    /// Gets or sets whether to validate the property as a URL.
    /// Adds [Url] attribute to the generated property.
    /// </summary>
    public bool Url { get; set; }

    /// <summary>
    /// Gets or sets whether to validate the property as a credit card number.
    /// Adds [CreditCard] attribute to the generated property.
    /// </summary>
    public bool CreditCard { get; set; }

    /// <summary>
    /// Gets or sets a custom error message for validation failures.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the name of another property to compare for equality.
    /// Useful for password confirmation scenarios.
    /// Adds [Compare] attribute to the generated property.
    /// </summary>
    public string? CompareProperty { get; set; }

    /// <summary>
    /// Gets or sets a custom validation type.
    /// The type must implement IValidatableObject or ValidationAttribute.
    /// </summary>
    public Type? CustomValidationType { get; set; }
}
