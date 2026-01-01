using Microsoft.CodeAnalysis;

namespace GenericDto.Analyzers.Extensions;

internal static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, string attributeFullName) => symbol.GetAttributes().Any(attr => attr.AttributeClass?.ToDisplayString() == attributeFullName);

    public static AttributeData? GetAttribute(this ISymbol symbol, string attributeFullName) => symbol.GetAttributes().FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == attributeFullName);

    public static bool IsReferenceType(this ITypeSymbol type) => type.IsReferenceType && type.SpecialType != SpecialType.System_String;

    public static bool IsValueType(this ITypeSymbol type) => type.IsValueType;

    public static bool IsNullable(this ITypeSymbol type) => type.NullableAnnotation == NullableAnnotation.Annotated;

    public static bool IsRecord(this INamedTypeSymbol type) => type.IsRecord;

    public static Accessibility GetEffectiveAccessibility(this ISymbol symbol) => symbol.DeclaredAccessibility;

    public static bool IsAccessibleFrom(this ISymbol symbol, ITypeSymbol fromType) => symbol.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal;

    public static string GetFullQualifiedName(this ITypeSymbol type, bool includeNullable = false)
    {
        var displayString = type.ToDisplayString(
            SymbolDisplayFormat.FullyQualifiedFormat
                .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
                .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier));

        return includeNullable ? displayString : displayString.TrimEnd('?');
    }

    public static T? GetNamedArgument<T>(this AttributeData attribute, string argumentName)
    {
        foreach (var namedArg in attribute.NamedArguments)
        {
            if (namedArg.Key == argumentName)
            {
                if (namedArg.Value.Value is T value)
                    return value;
                break;
            }
        }
        return default;
    }

    public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            foreach (var member in current.GetMembers())
            {
                yield return member;
            }
            current = current.BaseType;
        }
    }
}
