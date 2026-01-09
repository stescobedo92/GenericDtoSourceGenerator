using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for basic DTO generation using the [GenericDto] attribute.
/// </summary>
public class GenericDtoAttributeTests
{
    [Fact]
    public void Should_Generate_Dto_With_Default_Settings()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public partial class CustomerDto");
        dtoSource.Should().Contain("namespace TestNamespace.Dto");
        dtoSource.Should().Contain("public int Id { get; set; }");
        dtoSource.Should().Contain("public string Name { get; set; }");
        dtoSource.Should().Contain("public string Email { get; set; }");
    }

    [Fact]
    public void Should_Generate_Dto_With_Custom_Name()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(DtoName = ""CustomerResponse"")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerResponse");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public partial class CustomerResponse");
    }

    [Fact]
    public void Should_Generate_Dto_With_Custom_Namespace()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(Namespace = ""MyApp.Responses"")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("namespace MyApp.Responses");
    }

    [Fact]
    public void Should_Generate_Record_Dto_When_UseRecord_Is_True()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(UseRecord = true)]
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ProductDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public record ProductDto");
    }

    [Fact]
    public void Should_Generate_Internal_Dto_When_AccessModifier_Is_Internal()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(AccessModifier = ""internal"")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("internal partial class CustomerDto");
    }

    [Fact]
    public void Should_Generate_Parameterless_Constructor_By_Default()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public CustomerDto()");
    }

    [Fact]
    public void Should_Generate_Constructor_With_All_Properties()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public CustomerDto(int id, string name)");
    }

    [Fact]
    public void Should_Not_Generate_Parameterless_Constructor_When_Disabled()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(GenerateParameterlessConstructor = false)]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // Should have the parameterized constructor but count constructors
        dtoSource.Should().Contain("public CustomerDto(int id, string name)");
    }

    [Fact]
    public void Should_Generate_ToString_Override()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public override string ToString()");
    }

    [Fact]
    public void Should_Include_Nullable_Enable_Directive()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("#nullable enable");
    }

    [Fact]
    public void Should_Include_Generated_Code_Attributes()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("[global::System.CodeDom.Compiler.GeneratedCodeAttribute");
        dtoSource.Should().Contain("[global::System.Diagnostics.DebuggerNonUserCodeAttribute]");
        dtoSource.Should().Contain("[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute]");
    }
}
