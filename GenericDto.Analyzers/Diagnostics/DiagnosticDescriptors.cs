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
}
