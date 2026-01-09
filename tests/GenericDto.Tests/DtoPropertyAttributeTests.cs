using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for property customization using the [DtoProperty] attribute.
/// </summary>
public class DtoPropertyAttributeTests
{
    [Fact]
    public void Should_Rename_Property_Using_Name_Parameter()
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
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public string FullName { get; set; }");
        dtoSource.Should().NotContain("public string Name { get; set; }");
    }

    [Fact]
    public void Should_Ignore_Property_Using_Ignore_Parameter()
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
        
        [DtoProperty(Ignore = true)]
        public string InternalCode { get; set; }
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
        dtoSource.Should().NotContain("InternalCode");
    }

    [Fact]
    public void Should_Apply_Default_Value_To_Property()
    {
        // Arrange
        var source = """
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
        
        [DtoProperty(DefaultValue = "\"Unknown\"")]
        public string Name { get; set; }
    }
}
""";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("\"Unknown\"");
    }

    [Fact]
    public void Should_Handle_Nullable_Properties()
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
        public string? NickName { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("NickName");
    }

    [Fact]
    public void Should_Handle_Multiple_Properties_With_Attributes()
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
        
        [DtoProperty(Name = ""CustomerEmail"")]
        public string Email { get; set; }
        
        [DtoProperty(Ignore = true)]
        public string Secret { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("CustomerName");
        dtoSource.Should().Contain("CustomerEmail");
        dtoSource.Should().NotContain("Secret");
    }

    [Fact]
    public void Should_Handle_Value_Types()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsProcessed { get; set; }
        public Guid OrderGuid { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("int Id");
        dtoSource.Should().Contain("decimal Amount");
        dtoSource.Should().Contain("bool IsProcessed");
    }

    [Fact]
    public void Should_Skip_Static_Properties()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public static string DefaultName { get; set; } = ""Default"";
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
        dtoSource.Should().NotContain("DefaultName");
        dtoSource.Should().Contain("public int Id { get; set; }");
    }

    [Fact]
    public void Should_Skip_Non_Public_Properties()
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
        private string Secret { get; set; }
        protected string ProtectedValue { get; set; }
        internal string InternalValue { get; set; }
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
        dtoSource.Should().NotContain("Secret");
        dtoSource.Should().NotContain("ProtectedValue");
        dtoSource.Should().NotContain("InternalValue");
    }

    [Fact]
    public void Should_Include_Required_Attribute_For_NonNullable_Reference_Types()
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
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.Required]");
    }
}
