using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for null handling of optional properties and validation attribute generation.
/// </summary>
public class NullHandlingAndValidationTests
{
    [Fact]
    public void Should_Handle_Null_Name_Property()
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
        
        [DtoProperty(Name = null)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // When Name is null, should fallback to original property name
        dtoSource.Should().Contain("public string Name { get; set; }");
    }

    [Fact]
    public void Should_Handle_Empty_String_Name_Property()
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
        
        [DtoProperty(Name = """")]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // When Name is empty string, should fallback to original property name
        dtoSource.Should().Contain("public string Name { get; set; }");
    }

    [Fact]
    public void Should_Handle_Whitespace_Name_Property()
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
        
        [DtoProperty(Name = ""   "")]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // When Name is whitespace, should fallback to original property name
        dtoSource.Should().Contain("public string Name { get; set; }");
    }

    [Fact]
    public void Should_Handle_Null_DefaultValue_Property()
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
        
        [DtoProperty(DefaultValue = null)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // When DefaultValue is null, should use default initialization
        dtoSource.Should().Contain("public string Name { get; set; } = string.Empty;");
    }

    [Fact]
    public void Should_Generate_MaxLength_Attribute()
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
        
        [DtoProperty(MaxLength = 100)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.MaxLength(100)]");
    }

    [Fact]
    public void Should_Generate_MinLength_Attribute()
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
        
        [DtoProperty(MinLength = 3)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.MinLength(3)]");
    }

    [Fact]
    public void Should_Generate_RegularExpression_Attribute()
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
        
        [DtoProperty(Pattern = @"^[a-zA-Z]+$")]
        public string Name { get; set; }
    }
}
""";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.RegularExpression(@\"^[a-zA-Z]+$\")]");
    }

    [Fact]
    public void Should_Handle_Null_Pattern_Property()
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
        
        [DtoProperty(Pattern = null)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // When Pattern is null, should not generate RegularExpression attribute
        dtoSource.Should().NotContain("RegularExpression");
    }

    [Fact]
    public void Should_Generate_Range_Attribute()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Product
    {
        public int Id { get; set; }
        
        [DtoProperty(MinValue = 0.0, MaxValue = 100.0)]
        public double Price { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ProductDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.Range(0, 100)]");
    }

    [Fact]
    public void Should_Use_Custom_Description()
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
        
        [DtoProperty(Description = ""The customer's full name"")]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("/// The customer's full name");
    }

    [Fact]
    public void Should_Handle_Null_Description_Property()
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
        
        [DtoProperty(Description = null)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // When Description is null, should use auto-generated documentation
        dtoSource.Should().Contain("/// Gets or sets the Name value.");
    }

    [Fact]
    public void Should_Generate_Multiple_Validation_Attributes()
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
        
        [DtoProperty(MinLength = 3, MaxLength = 100, Pattern = @"^[a-zA-Z\s]+$", Description = "Customer name")]
        public string Name { get; set; }
    }
}
""";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("/// Customer name");
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.Required]");
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.MaxLength(100)]");
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.MinLength(3)]");
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.RegularExpression(@\"^[a-zA-Z\\s]+$\")]");
    }

    [Fact]
    public void Should_Not_Generate_Validation_Attributes_For_Default_Values()
    {
        // Arrange
        // Note: The extreme double values below represent double.MinValue and double.MaxValue
        var source = $@"
using GenericDto.Core.Attributes;

namespace TestNamespace
{{
    [GenericDto]
    public class Product
    {{
        public int Id {{ get; set; }}
        
        [DtoProperty(MaxLength = -1, MinLength = -1)]
        public string Name {{ get; set; }}
        
        [DtoProperty(MinValue = {double.MinValue}, MaxValue = {double.MaxValue})]
        public double Price {{ get; set; }}
    }}
}}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ProductDto.g.cs");
        dtoSource.Should().NotBeNull();
        // When MaxLength/MinLength are -1 (default), should not generate attributes
        dtoSource.Should().NotContain("[global::System.ComponentModel.DataAnnotations.MaxLength(-1)]");
        dtoSource.Should().NotContain("[global::System.ComponentModel.DataAnnotations.MinLength(-1)]");
        // When MinValue/MaxValue are at their extremes, they should not generate Range attribute
        dtoSource.Should().NotContain("Range");
    }

    [Fact]
    public void Should_Handle_Pattern_With_Special_Characters()
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
        
        [DtoProperty(Pattern = @"^\d{3}-\d{3}-\d{4}$")]
        public string PhoneNumber { get; set; }
    }
}
""";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain(@"[global::System.ComponentModel.DataAnnotations.RegularExpression(@""^\d{3}-\d{3}-\d{4}$"")]");
    }

    [Fact]
    public void Should_Escape_Pattern_With_Backslashes_And_Quotes()
    {
        // Arrange - Testing a pattern with both backslashes and quotes
        var source = """
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        public int Id { get; set; }
        
        [DtoProperty(Pattern = @"^[\w\.\-]+@[\w\.\-]+\.\w+$")]
        public string Email { get; set; }
    }
}
""";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("RegularExpression");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_String_Validation_On_NonString_Property()
    {
        // Arrange
        var source = """
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public int Id { get; set; }
        
        [DtoProperty(MaxLength = 10, Pattern = ".*")]
        public int Quantity { get; set; }
    }
}
""";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        diagnostics.Should().Contain(d => d.Id == "DTO006");

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().NotContain("MaxLength");
        dtoSource.Should().NotContain("RegularExpression");
    }

    [Fact]
    public void Should_Report_Diagnostic_For_Numeric_Validation_On_NonNumeric_Property()
    {
        // Arrange
        var source = """
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Product
    {
        public int Id { get; set; }
        
        [DtoProperty(MinValue = 0, MaxValue = 10)]
        public string Description { get; set; }
    }
}
""";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        diagnostics.Should().Contain(d => d.Id == "DTO007");

        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ProductDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().NotContain("Range(");
    }
}
