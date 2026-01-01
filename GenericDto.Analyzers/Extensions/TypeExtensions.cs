using Microsoft.CodeAnalysis;

namespace GenericDto.Analyzers.Extensions;

internal static class TypeExtensions
{
    public static string GetSafeTypeName(this ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arrayType)
        {
            return $"{arrayType.ElementType.GetSafeTypeName()}[]";
        }

        if (type is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            var typeName = namedType.ConstructedFrom.Name;
            var typeArgs = string.Join(", ", namedType.TypeArguments.Select(t => t.GetSafeTypeName()));
            return $"{typeName}<{typeArgs}>";
        }

        return type.Name;
    }

    public static bool IsCollectionType(this ITypeSymbol type)
    {
        return type.AllInterfaces.Any(i =>
            i.ToDisplayString() == "System.Collections.IEnumerable" ||
            i.ToDisplayString() == "System.Collections.Generic.IEnumerable<T>");
    }

    public static bool IsDictionaryType(this ITypeSymbol type)
    {
        return type.AllInterfaces.Any(i =>
            i.Name.StartsWith("IDictionary") ||
            i.Name.StartsWith("IReadOnlyDictionary"));
    }

    public static bool IsSimpleType(this ITypeSymbol type)
    {
        // SpecialType does not include DateTimeOffset, Guid, or TimeSpan.
        // Handle them by checking the fully qualified name.
        if (type.SpecialType switch
        {
            SpecialType.System_Boolean or
            SpecialType.System_Char or
            SpecialType.System_SByte or
            SpecialType.System_Byte or
            SpecialType.System_Int16 or
            SpecialType.System_UInt16 or
            SpecialType.System_Int32 or
            SpecialType.System_UInt32 or
            SpecialType.System_Int64 or
            SpecialType.System_UInt64 or
            SpecialType.System_Single or
            SpecialType.System_Double or
            SpecialType.System_Decimal or
            SpecialType.System_String or
            SpecialType.System_DateTime => true,
            _ => false
        })
        {
            return true;
        }

        var fullName = type.ToDisplayString();

        return fullName == "System.DateTimeOffset"
            || fullName == "System.Guid"
            || fullName == "System.TimeSpan";
    }

    public static ITypeSymbol? GetCollectionElementType(this ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arrayType)
            return arrayType.ElementType;

        return type.AllInterfaces.FirstOrDefault(i => i.Name == "IEnumerable" && i.IsGenericType)?.TypeArguments.FirstOrDefault();
    }
}
