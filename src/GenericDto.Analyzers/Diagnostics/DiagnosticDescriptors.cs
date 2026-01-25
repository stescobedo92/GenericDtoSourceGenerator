using Microsoft.CodeAnalysis;

namespace GenericDto.Analyzers.Diagnostics;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor MissingGenerateDtoAttribute = new(
        id: DiagnosticIds.DTO001,
        title: "GenerateDto attribute is missing",
        messageFormat: "Class '{0}' must be marked with [GenerateDto] attribute",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The class must be decorated with the GenerateDto attribute to generate a DTO."
    );

    public static readonly DiagnosticDescriptor InvalidDtoConfiguration = new(
        id: DiagnosticIds.DTO002,
        title: "Invalid DTO configuration",
        messageFormat: "Invalid configuration for DTO generation: {0}",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The DTO generation configuration is invalid."
    );

    public static readonly DiagnosticDescriptor DuplicateDtoName = new(
        id: DiagnosticIds.DTO003,
        title: "Duplicate DTO name",
        messageFormat: "Multiple DTOs with the name '{0}' would be generated. Use DtoName parameter to specify unique names.",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "DTO names must be unique within the same namespace."
    );

    public static readonly DiagnosticDescriptor InvalidPropertyType = new(
        id: DiagnosticIds.DTO004,
        title: "Invalid property type",
        messageFormat: "Property '{0}' has an invalid type '{1}' that cannot be used in DTO generation",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Some property types may not be suitable for DTOs."
    );

    public static readonly DiagnosticDescriptor CircularReference = new(
        id: DiagnosticIds.DTO005,
        title: "Circular reference detected",
        messageFormat: "Circular reference detected between '{0}' and '{1}'",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Circular references may cause serialization issues."
    );

    public static readonly DiagnosticDescriptor StringValidationOnNonStringProperty = new(
        id: DiagnosticIds.DTO006,
        title: "String validation attribute applied to non-string property",
        messageFormat: "Property '{0}' of type '{1}' cannot use string validation attributes (MaxLength, MinLength, or Pattern). These attributes are only valid for string properties.",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "String validation attributes like MaxLength, MinLength, and Pattern should only be applied to string properties."
    );

    public static readonly DiagnosticDescriptor NumericValidationOnNonNumericProperty = new(
        id: DiagnosticIds.DTO007,
        title: "Numeric validation attribute applied to non-numeric property",
        messageFormat: "Property '{0}' of type '{1}' cannot use numeric validation attributes (MinValue or MaxValue). These attributes are only valid for numeric properties.",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Numeric validation attributes like MinValue and MaxValue should only be applied to numeric properties (int, long, double, decimal, etc.)."
    );

    public static readonly DiagnosticDescriptor InvalidStringValidationRange = new(
        id: DiagnosticIds.DTO008,
        title: "Invalid string validation range",
        messageFormat: "Property '{0}' has invalid string length range: MinLength ({1}) cannot be greater than MaxLength ({2}).",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "MinLength must be less than or equal to MaxLength when both are specified."
    );

    public static readonly DiagnosticDescriptor InvalidNumericValidationRange = new(
        id: DiagnosticIds.DTO009,
        title: "Invalid numeric validation range",
        messageFormat: "Property '{0}' has invalid numeric range: MinValue ({1}) cannot be greater than MaxValue ({2}).",
        category: "GenerateDto",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "MinValue must be less than or equal to MaxValue when both are specified."
    );
}
