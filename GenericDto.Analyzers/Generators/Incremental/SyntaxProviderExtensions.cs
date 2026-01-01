using GenericDto.Analyzers.Constants;
using GenericDto.Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenericDto.Analyzers.Generators.Incremental;

internal static class SyntaxProviderExtensions
{
    public static IncrementalValuesProvider<TypeDeclarationSyntax> GetAnnotatedTypes(this IncrementalGeneratorInitializationContext context, string attributeFullName)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => IsTypeDeclarationWithAttribute(node, attributeFullName),
                transform: (ctx, _) => GetTypeDeclarationSyntax(ctx))
            .Where(type => type is not null)!;
    }

    private static bool IsTypeDeclarationWithAttribute(SyntaxNode node, string attributeFullName)
    {
        return node is TypeDeclarationSyntax typeDecl &&
               typeDecl.AttributeLists.Any(attrList =>
                   attrList.Attributes.Any(attr =>
                       attr.Name.ToString().Contains("GenerateDto") ||
                       attr.Name.ToString().Contains("GenerateDtoAttribute")));
    }

    private static TypeDeclarationSyntax? GetTypeDeclarationSyntax(GeneratorSyntaxContext context)
    {
        var typeDecl = (TypeDeclarationSyntax)context.Node;

        // Check if the semantic model has the attribute
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(typeDecl);
        if (typeSymbol is null)
            return null;

        if (!typeSymbol.HasAttribute(GeneratorConstants.GenericDtoCoreAttributeFullName))
            return null;

        return typeDecl;
    }
}
