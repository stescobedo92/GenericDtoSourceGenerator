using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for property exclusion using the [DtoIgnore] attribute.
/// </summary>
public class DtoIgnoreAttributeTests
{
    [Fact]
    public void Should_Exclude_Property_With_DtoIgnore_Attribute()
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
        
        [DtoIgnore]
        public string Password { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public int Id { get; set; }");
        dtoSource.Should().Contain("public string Name { get; set; }");
        dtoSource.Should().NotContain("Password");
    }

    [Fact]
    public void Should_Exclude_Multiple_Properties_With_DtoIgnore_Attribute()
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
        
        [DtoIgnore]
        public string Password { get; set; }
        
        [DtoIgnore]
        public string InternalCode { get; set; }
        
        [DtoIgnore]
        public decimal SensitiveData { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public int Id { get; set; }");
        dtoSource.Should().Contain("public string Name { get; set; }");
        dtoSource.Should().NotContain("Password");
        dtoSource.Should().NotContain("InternalCode");
        dtoSource.Should().NotContain("SensitiveData");
    }

    [Fact]
    public void Should_Not_Include_Ignored_Properties_In_Constructor()
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
        
        [DtoIgnore]
        public string Password { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // Constructor should only have Id and Name, not Password
        dtoSource.Should().Contain("public CustomerDto(int id, string name)");
        dtoSource.Should().NotContain("password");
    }

    [Fact]
    public void Should_Not_Include_Ignored_Properties_In_Mapper()
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
        
        [DtoIgnore]
        public string Password { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("Id = source.Id");
        mapperSource.Should().Contain("Name = source.Name");
        mapperSource.Should().NotContain("Password");
    }

    [Fact]
    public void Should_Handle_All_Properties_Ignored_Except_One()
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
        
        [DtoIgnore]
        public string Name { get; set; }
        
        [DtoIgnore]
        public string Email { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public int Id { get; set; }");
        dtoSource.Should().NotContain("public string Name");
        dtoSource.Should().NotContain("public string Email");
    }

    [Fact]
    public void Should_Work_With_DtoIgnore_And_DtoProperty_Together()
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
        
        [DtoProperty(Name = ""CustomerName"")]
        public string Name { get; set; }
        
        [DtoIgnore]
        public string Password { get; set; }
        
        [DtoProperty(Name = ""CustomerEmail"")]
        public string Email { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public int Id { get; set; }");
        dtoSource.Should().Contain("CustomerName");
        dtoSource.Should().Contain("CustomerEmail");
        dtoSource.Should().NotContain("Password");
    }
}
