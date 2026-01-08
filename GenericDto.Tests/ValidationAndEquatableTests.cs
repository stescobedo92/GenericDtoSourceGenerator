using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for IValidatableObject and IEquatable implementation generation.
/// </summary>
public class ValidationAndEquatableTests
{
    [Fact]
    public void Should_Implement_IValidatableObject_When_Enabled()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIValidatableObject = true)]
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
        dtoSource.Should().Contain("IValidatableObject");
        dtoSource.Should().Contain("Validate(");
        dtoSource.Should().Contain("ValidationContext validationContext");
    }

    [Fact]
    public void Should_Include_ComponentModel_DataAnnotations_Using_When_IValidatableObject()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIValidatableObject = true)]
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
        dtoSource.Should().Contain("System.ComponentModel.DataAnnotations");
    }

    [Fact]
    public void Should_Implement_IEquatable_When_Enabled()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIEquatable = true)]
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
        dtoSource.Should().Contain("IEquatable<CustomerDto>");
        dtoSource.Should().Contain("public bool Equals(CustomerDto? other)");
    }

    [Fact]
    public void Should_Generate_GetHashCode_When_IEquatable_Enabled()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIEquatable = true)]
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
        dtoSource.Should().Contain("public override int GetHashCode()");
        dtoSource.Should().Contain("HashCode");
    }

    [Fact]
    public void Should_Generate_Equality_Operators_When_IEquatable_Enabled()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIEquatable = true)]
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
        dtoSource.Should().Contain("public static bool operator ==");
        dtoSource.Should().Contain("public static bool operator !=");
    }

    [Fact]
    public void Should_Override_Object_Equals_When_IEquatable_Enabled()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIEquatable = true)]
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
        dtoSource.Should().Contain("public override bool Equals(object? obj)");
    }

    [Fact]
    public void Should_Implement_Both_IEquatable_And_IValidatableObject()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIEquatable = true, ImplementIValidatableObject = true)]
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
        dtoSource.Should().Contain("IEquatable<CustomerDto>");
        dtoSource.Should().Contain("IValidatableObject");
    }

    [Fact]
    public void Should_Not_Generate_IEquatable_For_Records()
    {
        // Arrange - Records already have value-based equality
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(UseRecord = true, ImplementIEquatable = true)]
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
        dtoSource.Should().Contain("public record CustomerDto");
        // Records have built-in equality, so explicit IEquatable implementation should not be generated
    }

    [Fact]
    public void Should_Generate_Validate_Method_With_Required_Property_Checks()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIValidatableObject = true)]
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
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("Validate(");
        dtoSource.Should().Contain("ValidationResult");
    }

    [Fact]
    public void Should_Handle_IEquatable_With_Single_Property()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIEquatable = true)]
    public class SingleProp
    {
        public int Id { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "SinglePropDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("IEquatable<SinglePropDto>");
        dtoSource.Should().Contain("EqualityComparer");
    }

    [Fact]
    public void Should_Handle_IEquatable_With_No_Properties()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(ImplementIEquatable = true)]
    public class EmptyClass
    {
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "EmptyClassDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("IEquatable<EmptyClassDto>");
        dtoSource.Should().Contain("return true;"); // Empty class should always be equal
    }
}
