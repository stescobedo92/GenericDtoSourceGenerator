using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for mapper extension method generation.
/// </summary>
public class MapperGenerationTests
{
    [Fact]
    public void Should_Generate_ToDto_Extension_Method()
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
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("public static CustomerDto ToDto(this TestNamespace.Customer source)");
    }

    [Fact]
    public void Should_Generate_ToEntity_Extension_Method()
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
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("public static TestNamespace.Customer ToEntity(this CustomerDto dto)");
    }

    [Fact]
    public void Should_Generate_ToDtoList_Extension_Method()
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
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("ToDtoList");
        mapperSource.Should().Contain("IEnumerable<TestNamespace.Customer>");
    }

    [Fact]
    public void Should_Generate_ToEntityList_Extension_Method()
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
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("ToEntityList");
        mapperSource.Should().Contain("IEnumerable<CustomerDto>");
    }

    [Fact]
    public void Should_Generate_UpdateFrom_Extension_Method()
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
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("public static TestNamespace.Customer UpdateFrom(this TestNamespace.Customer entity, CustomerDto dto)");
    }

    [Fact]
    public void Should_Include_Null_Checks_In_Mapper_Methods()
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
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("throw new global::System.ArgumentNullException(nameof(source))");
        mapperSource.Should().Contain("throw new global::System.ArgumentNullException(nameof(dto))");
    }

    [Fact]
    public void Should_Generate_Mapper_Class_With_Static_Extensions()
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
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("public static class CustomerDtoMapperExtensions");
    }

    [Fact]
    public void Should_Map_All_Non_Ignored_Properties_In_ToDto()
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
        
        [DtoIgnore]
        public string Secret { get; set; }
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
        mapperSource.Should().Contain("Email = source.Email");
        mapperSource.Should().NotContain("Secret = source.Secret");
    }

    [Fact]
    public void Should_Handle_Renamed_Properties_In_Mapper()
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
        
        [DtoProperty(Name = ""FullName"")]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        // In ToDto: FullName = source.Name
        mapperSource.Should().Contain("FullName = source.Name");
    }

    [Fact]
    public void Should_Generate_Mapper_In_Same_Namespace_As_Dto()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(Namespace = ""CustomNamespace"")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("namespace CustomNamespace");
    }

    [Fact]
    public void Should_Include_XML_Documentation_In_Mapper_Methods()
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
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("/// <summary>");
        mapperSource.Should().Contain("/// <param name=");
        mapperSource.Should().Contain("/// <returns>");
    }
}
