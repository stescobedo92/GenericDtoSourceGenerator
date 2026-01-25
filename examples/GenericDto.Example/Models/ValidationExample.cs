using GenericDto.Core.Attributes;

namespace GenericDto.Example.Models;

/// <summary>
/// Example demonstrating validation attribute type checking.
/// This class intentionally contains both valid and invalid attribute usage
/// to showcase the new validation warnings.
/// </summary>
public class ValidationExample
{
    // Valid string validations - no warnings
    [DtoProperty(MaxLength = 100)]
    public string Name { get; set; } = string.Empty;

    [DtoProperty(MinLength = 5, MaxLength = 50)]
    public string Email { get; set; } = string.Empty;

    [DtoProperty(Pattern = @"^\d{3}-\d{3}-\d{4}$")]
    public string PhoneNumber { get; set; } = string.Empty;

    // Valid numeric validations - no warnings
    [DtoProperty(MinValue = 0, MaxValue = 120)]
    public int Age { get; set; }

    [DtoProperty(MinValue = 0.0, MaxValue = 9999.99)]
    public decimal Salary { get; set; }

    [DtoProperty(MinValue = 0, MaxValue = 100)]
    public int? OptionalPercentage { get; set; }

    // INVALID: String validation on non-string properties - will generate DTO006 warnings
    // Uncomment to see the warnings:
    
    // [DtoProperty(MaxLength = 50)]
    // public int InvalidMaxLength { get; set; }

    // [DtoProperty(MinLength = 10)]
    // public bool InvalidMinLength { get; set; }

    // [DtoProperty(Pattern = "[0-9]+")]
    // public decimal InvalidPattern { get; set; }

    // INVALID: Numeric validation on non-numeric properties - will generate DTO007 warnings
    // Uncomment to see the warnings:

    // [DtoProperty(MinValue = 0, MaxValue = 100)]
    // public string InvalidRange { get; set; }

    // [DtoProperty(MinValue = 1)]
    // public bool InvalidMinValue { get; set; }

    // [DtoProperty(MaxValue = 1000)]
    // public DateTime InvalidMaxValue { get; set; }
}
