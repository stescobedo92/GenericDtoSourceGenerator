using System.Linq;
using GenericDto.Analyzers.Constants;
using GenericDto.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenericDto.Analyzers.Generators.Incremental;

/// <summary>
/// Extension methods for working with IncrementalGeneratorInitializationContext.
/// </summary>
internal static class SyntaxProviderExtensions
{
    /// <summary>
    /// Gets types annotated with the specified attribute.
    /// </summary>
    public static IncrementalValuesProvider<TypeDeclarationSyntax> GetAnnotatedTypes(
        this IncrementalGeneratorInitializationContext context,
        string attributeFullName)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsTypeDeclarationWithAttribute(node),
                transform: static (ctx, _) => GetTypeDeclarationSyntax(ctx))
            .Where(static type => type is not null)!;
    }

    private static bool IsTypeDeclarationWithAttribute(SyntaxNode node)
    {
        return node is TypeDeclarationSyntax typeDecl &&
               typeDecl.AttributeLists.Count > 0 &&
               typeDecl.AttributeLists.Any(attrList =>
                   attrList.Attributes.Any(attr =>
                       attr.Name.ToString().Contains("GenericDto")));
    }

    private static TypeDeclarationSyntax? GetTypeDeclarationSyntax(GeneratorSyntaxContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;

        // Verify the semantic model has the attribute
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);
        if (typeSymbol is null)
            return null;

        if (!typeSymbol.HasAttribute(GeneratorConstants.GenericDtoCoreAttributeFullName))
            return null;

        return typeDecl;
    }
}
