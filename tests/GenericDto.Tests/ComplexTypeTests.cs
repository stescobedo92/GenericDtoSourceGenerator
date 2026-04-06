using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for complex type support via the Type property of DtoPropertyAttribute.
/// </summary>
public class ComplexTypeTests
{
    [Fact]
    public void Should_Generate_Collection_Type_Override_Property()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public int Id { get; set; }

        [DtoProperty(Type = typeof(List<string>))]
        public string Tags { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("List<string>");
        dtoSource.Should().NotContain("public string Tags");
    }

    [Fact]
    public void Should_Generate_Generic_Dictionary_Type_Override_Property()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Config
    {
        public int Id { get; set; }

        [DtoProperty(Type = typeof(Dictionary<string, int>))]
        public string Settings { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ConfigDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("Dictionary<string, int>");
    }

    [Fact]
    public void Should_Set_IsRequired_Based_On_Effective_Type_Not_Source_Type()
    {
        // Arrange: original property is a value type (int), overridden to reference type (string)
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Product
    {
        [DtoProperty(Type = typeof(string))]
        public int Code { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ProductDto.g.cs");
        dtoSource.Should().NotBeNull();
        // Overriding to a non-nullable reference type means the property is required
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.Required]");
    }

    [Fact]
    public void Should_Not_Set_IsRequired_When_Effective_Type_Is_Value_Type()
    {
        // Arrange: original property is a reference type (string), overridden to value type (int)
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }

        [DtoProperty(Type = typeof(int))]
        public string Code { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // Value type override means it cannot be null and is not "required" in the DataAnnotations sense
        dtoSource.Should().Contain("public int Code { get; set; }");
        // int is a value type - should not have [Required]
        dtoSource.Should().NotContain("[global::System.ComponentModel.DataAnnotations.Required]\r\npublic int Code");
        dtoSource.Should().NotContain("[global::System.ComponentModel.DataAnnotations.Required]\npublic int Code");
    }

    [Fact]
    public void Should_Skip_Mapper_Assignment_And_Add_Comment_When_Type_Is_Overridden_In_ToDto()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public int Id { get; set; }

        [DtoProperty(Type = typeof(List<string>))]
        public string Tags { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDtoMapper.g.cs");
        mapperSource.Should().NotBeNull();
        // The overridden property must not be directly assigned (would cause compile error)
        mapperSource.Should().NotContain("Tags = source.Tags,");
        // A helpful comment should be present instead
        mapperSource.Should().Contain("// Tags:");
        mapperSource.Should().Contain("manual mapping");
    }

    [Fact]
    public void Should_Skip_Mapper_Assignment_And_Add_Comment_When_Type_Is_Overridden_In_ToEntity()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public int Id { get; set; }

        [DtoProperty(Type = typeof(List<string>))]
        public string Tags { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDtoMapper.g.cs");
        mapperSource.Should().NotBeNull();
        // The overridden property must not be directly assigned in ToEntity (would cause compile error)
        mapperSource.Should().NotContain("Tags = dto.Tags,");
        // A helpful comment should be present instead
        mapperSource.Should().Contain("// Tags:");
        mapperSource.Should().Contain("manual mapping");
    }

    [Fact]
    public void Should_Skip_Mapper_Assignment_And_Add_Comment_When_Type_Is_Overridden_In_UpdateFrom()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public int Id { get; set; }

        [DtoProperty(Type = typeof(List<string>))]
        public string Tags { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDtoMapper.g.cs");
        mapperSource.Should().NotBeNull();
        // The overridden property must not be directly assigned in UpdateFrom (would cause compile error)
        mapperSource.Should().NotContain("entity.Tags = dto.Tags;");
        // A helpful comment should be present instead
        mapperSource.Should().Contain("// Tags:");
        mapperSource.Should().Contain("manual mapping");
    }

    [Fact]
    public void Should_Map_Non_Overridden_Properties_Normally_When_Some_Properties_Have_Type_Override()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DtoProperty(Type = typeof(List<string>))]
        public string Tags { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDtoMapper.g.cs");
        mapperSource.Should().NotBeNull();
        // Non-overridden properties should still be mapped
        mapperSource.Should().Contain("Id = source.Id,");
        mapperSource.Should().Contain("Name = source.Name,");
        // Overridden property skipped with comment
        mapperSource.Should().NotContain("Tags = source.Tags,");
    }

    [Fact]
    public void Should_Use_Fully_Qualified_Name_For_Generic_Collection_Override()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Product
    {
        public int Id { get; set; }

        [DtoProperty(Type = typeof(IList<decimal>))]
        public string Prices { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ProductDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("IList<decimal>");
        dtoSource.Should().NotContain("public string Prices");
    }

    [Fact]
    public void Should_Correctly_Override_Type_While_Preserving_Property_Name()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }

        [DtoProperty(Name = ""ContactNumbers"", Type = typeof(List<string>))]
        public string Phone { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("ContactNumbers");
        dtoSource.Should().Contain("List<string>");
        dtoSource.Should().NotContain("public string ContactNumbers");
        dtoSource.Should().NotContain("Phone");
    }

    [Fact]
    public void Should_Not_Add_Comment_When_Type_Is_Not_Overridden()
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

        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDtoMapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().NotContain("manual mapping");
        mapperSource.Should().Contain("Id = source.Id,");
        mapperSource.Should().Contain("Name = source.Name,");
    }
}
