using System.Collections.Immutable;
using System.Reflection;
using GenericDto.Analyzers.Generators;
using GenericDto.Core.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenericDto.Tests.Helpers;

/// <summary>
/// Helper class for testing source generators.
/// </summary>
public static class SourceGeneratorTestHelper
{
    /// <summary>
    /// Runs the DtoSourceGenerator on the provided source code.
    /// </summary>
    /// <param name="source">The source code to compile and run the generator on.</param>
    /// <returns>A tuple containing the compilation output, generated sources, and diagnostics.</returns>
    public static (Compilation OutputCompilation, ImmutableArray<GeneratedSourceResult> GeneratedSources, ImmutableArray<Diagnostic> Diagnostics) RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Get references from the runtime assemblies
        var references = GetMetadataReferences();

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable));

        var generator = new DtoSourceGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var runResult = driver.GetRunResult();

        var generatedSources = new List<GeneratedSourceResult>();
        if (runResult.Results.Length > 0)
        {
            for (int i = 0; i < runResult.GeneratedTrees.Length; i++)
            {
                generatedSources.Add(new GeneratedSourceResult(
                    runResult.Results[0].GeneratedSources[i].HintName,
                    runResult.GeneratedTrees[i].GetText().ToString()));
            }
        }

        return (outputCompilation, generatedSources.ToImmutableArray(), diagnostics);
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        // Get the directory of the runtime
        var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        
        var references = new List<MetadataReference>
        {
            // Core runtime references
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(GenericDtoAttribute).Assembly.Location),
            
            // Additional runtime references needed for compilation
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.dll")),
        };

        // Try to add ComponentModel.Annotations if available
        var annotationsPath = Path.Combine(assemblyPath, "System.ComponentModel.Annotations.dll");
        if (File.Exists(annotationsPath))
        {
            references.Add(MetadataReference.CreateFromFile(annotationsPath));
        }

        // Try to load netstandard if needed
        try
        {
            var netStandardAssembly = Assembly.Load("netstandard");
            references.Add(MetadataReference.CreateFromFile(netStandardAssembly.Location));
        }
        catch
        {
            // Ignore if not available
        }

        return references;
    }

    /// <summary>
    /// Gets the generated source code by hint name.
    /// </summary>
    public static string? GetGeneratedSource(ImmutableArray<GeneratedSourceResult> sources, string hintNamePart)
    {
        var result = sources.FirstOrDefault(s => s.HintName.Contains(hintNamePart));
        return result.SourceText; // Will return null if not found (default struct)
    }

    /// <summary>
    /// Verifies that the compilation has no errors.
    /// </summary>
    public static void VerifyNoCompilationErrors(Compilation compilation)
    {
        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (errors.Count > 0)
        {
            var errorMessages = string.Join(Environment.NewLine, errors.Select(e => e.ToString()));
            throw new InvalidOperationException($"Compilation errors found:{Environment.NewLine}{errorMessages}");
        }
    }
}

/// <summary>
/// Represents a generated source result.
/// </summary>
public record GeneratedSourceResult(string HintName, string SourceText);
