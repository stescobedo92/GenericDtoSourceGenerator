using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

/// <summary>
/// Tests for edge cases and special scenarios.
/// </summary>
public class EdgeCaseTests
{
    [Fact]
    public void Should_Handle_Class_With_No_Properties()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
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
        dtoSource.Should().Contain("public partial class EmptyClassDto");
    }

    [Fact]
    public void Should_Handle_Class_With_Only_Ignored_Properties()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        [DtoIgnore]
        public int Id { get; set; }
        
        [DtoIgnore]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public partial class CustomerDto");
    }

    [Fact]
    public void Should_Handle_Nested_Namespace()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace Company.Department.Project.Models
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
        dtoSource.Should().Contain("namespace Company.Department.Project.Models.Dto");
    }

    [Fact]
    public void Should_Handle_Properties_With_Special_Types()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System;
using System.Collections.Generic;

namespace TestNamespace
{
    [GenericDto]
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public TimeSpan Duration { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("OrderId");
        dtoSource.Should().Contain("OrderDate");
        dtoSource.Should().Contain("Duration");
    }

    [Fact]
    public void Should_Handle_ReadOnly_Properties()
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
        public string Name { get; }
        public string FullName => $""{Name}"";
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        // Read-only properties should still be included in DTO
        dtoSource.Should().Contain("Name");
    }

    [Fact]
    public void Should_Handle_Struct_With_GenericDto()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "PointDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public int X { get; set; }");
        dtoSource.Should().Contain("public int Y { get; set; }");
    }

    [Fact]
    public void Should_Handle_Multiple_Classes_With_GenericDto()
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

    [GenericDto]
    public class Order
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    [GenericDto]
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var customerDtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        var orderDtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDto.g.cs");
        var productDtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "ProductDto.g.cs");
        
        customerDtoSource.Should().NotBeNull();
        orderDtoSource.Should().NotBeNull();
        productDtoSource.Should().NotBeNull();
    }

    [Fact]
    public void Should_Handle_Class_With_Single_Property()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
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
        dtoSource.Should().Contain("public int Id { get; set; }");
        dtoSource.Should().Contain("public SinglePropDto(int id)");
    }

    [Fact]
    public void Should_Handle_Nullable_Value_Types()
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
        public int? Quantity { get; set; }
        public decimal? Discount { get; set; }
        public DateTime? ShippedDate { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "OrderDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("Quantity");
        dtoSource.Should().Contain("Discount");
        dtoSource.Should().Contain("ShippedDate");
    }

    [Fact]
    public void Should_Generate_Correct_ToString_With_Multiple_Properties()
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
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public override string ToString()");
        dtoSource.Should().Contain("CustomerDto");
    }

    [Fact]
    public void Should_Handle_Generic_Class_Name_Conflicts()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(DtoName = ""CustomerDto"")]
    public class CustomerEntity
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
        dtoSource.Should().Contain("public partial class CustomerDto");
    }
}
