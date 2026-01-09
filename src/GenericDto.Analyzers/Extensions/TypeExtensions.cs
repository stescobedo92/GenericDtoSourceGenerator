using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace GenericDto.Analyzers.Extensions;

/// <summary>
/// Extension methods for working with ITypeSymbol.
/// </summary>
internal static class TypeExtensions
{
    private static readonly HashSet<string> SimpleTypeNames = new(StringComparer.Ordinal)
    {
        "System.DateTimeOffset",
        "System.Guid",
        "System.TimeSpan",
        "System.Uri",
        "System.Version"
    };

    /// <summary>
    /// Gets a safe type name suitable for code generation.
    /// </summary>
    public static string GetSafeTypeName(this ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arrayType)
        {
            return $"{arrayType.ElementType.GetSafeTypeName()}[]";
        }

        if (type is INamedTypeSymbol namedType)
        {
            if (namedType.IsGenericType)
            {
                var typeName = namedType.ConstructedFrom.Name;
                var typeArgs = string.Join(", ", namedType.TypeArguments.Select(t => t.GetSafeTypeName()));
                return $"{typeName}<{typeArgs}>";
            }

            // Handle nullable value types
            if (namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                return $"{namedType.TypeArguments[0].GetSafeTypeName()}?";
            }
        }

        return type.Name;
    }

    /// <summary>
    /// Gets the fully qualified type name for code generation.
    /// </summary>
    public static string GetFullyQualifiedTypeName(this ITypeSymbol type, bool includeNullability = true)
    {
        var format = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: includeNullability
                ? SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
                : SymbolDisplayMiscellaneousOptions.None);

        return type.ToDisplayString(format);
    }

    /// <summary>
    /// Checks if the type is a collection type (implements IEnumerable).
    /// </summary>
    public static bool IsCollectionType(this ITypeSymbol type)
    {
        // String implements IEnumerable<char> but we don't want to treat it as a collection
        if (type.SpecialType == SpecialType.System_String)
            return false;

        return type.AllInterfaces.Any(i =>
            i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T ||
            i.SpecialType == SpecialType.System_Collections_IEnumerable);
    }

    /// <summary>
    /// Checks if the type is a dictionary type.
    /// </summary>
    public static bool IsDictionaryType(this ITypeSymbol type)
    {
        return type.AllInterfaces.Any(i =>
            i.OriginalDefinition.ToDisplayString().StartsWith("System.Collections.Generic.IDictionary", StringComparison.Ordinal) ||
            i.OriginalDefinition.ToDisplayString().StartsWith("System.Collections.Generic.IReadOnlyDictionary", StringComparison.Ordinal));
    }

    /// <summary>
    /// Checks if the type is a simple/primitive type suitable for DTOs.
    /// </summary>
    public static bool IsSimpleType(this ITypeSymbol type)
    {
        // Check for built-in types via SpecialType
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_Char:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_String:
            case SpecialType.System_DateTime:
                return true;
        }

        // Check for other common simple types
        var fullName = type.ToDisplayString();
        return SimpleTypeNames.Contains(fullName);
    }

    /// <summary>
    /// Checks if the type is an enum.
    /// </summary>
    public static bool IsEnumType(this ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Enum;
    }

    /// <summary>
    /// Gets the element type from a collection.
    /// </summary>
    public static ITypeSymbol? GetCollectionElementType(this ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arrayType)
            return arrayType.ElementType;

        // Check for IEnumerable<T>
        var enumerableInterface = type.AllInterfaces
            .FirstOrDefault(i => i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);

        return enumerableInterface?.TypeArguments.FirstOrDefault();
    }

    /// <summary>
    /// Gets the key and value types from a dictionary.
    /// </summary>
    public static (ITypeSymbol? KeyType, ITypeSymbol? ValueType) GetDictionaryTypes(this ITypeSymbol type)
    {
        var dictInterface = type.AllInterfaces
            .FirstOrDefault(i => i.OriginalDefinition.ToDisplayString().StartsWith("System.Collections.Generic.IDictionary", StringComparison.Ordinal) ||
                                 i.OriginalDefinition.ToDisplayString().StartsWith("System.Collections.Generic.IReadOnlyDictionary", StringComparison.Ordinal));

        if (dictInterface != null && dictInterface.TypeArguments.Length >= 2)
        {
            return (dictInterface.TypeArguments[0], dictInterface.TypeArguments[1]);
        }

        return (null, null);
    }

    /// <summary>
    /// Checks if the type can be safely serialized to JSON.
    /// </summary>
    public static bool IsJsonSerializable(this ITypeSymbol type)
    {
        // Delegates, pointers, and ref structs are not JSON-serializable
        if (type.TypeKind == TypeKind.Delegate ||
            type.TypeKind == TypeKind.Pointer ||
            type.TypeKind == TypeKind.FunctionPointer)
        {
            return false;
        }

        if (type is INamedTypeSymbol namedType && namedType.IsRefLikeType)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if the type is a nullable value type.
    /// </summary>
    public static bool IsNullableValueType(this ITypeSymbol type)
    {
        return type is INamedTypeSymbol namedType &&
               namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }

    /// <summary>
    /// Gets the underlying type if the type is nullable, otherwise returns the type itself.
    /// </summary>
    public static ITypeSymbol GetUnderlyingType(this ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedType &&
            namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
        {
            return namedType.TypeArguments[0];
        }

        return type;
    }
}
