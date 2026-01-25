using GenericDto.Tests.Helpers;
using Microsoft.CodeAnalysis;

namespace GenericDto.Tests;

/// <summary>
/// Tests for validation attribute type checking.
/// Ensures that string validation attributes are only applied to string properties
/// and numeric validation attributes are only applied to numeric properties.
/// </summary>
public class ValidationAttributeTypeCheckTests
{
    #region String Validation on Non-String Properties

    [Fact]
    public void MaxLength_OnIntProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxLength = 50)]
        public int Age { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO006");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO006");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Warning);
        diagnostic.GetMessage().Should().Contain("Age");
        diagnostic.GetMessage().Should().Contain("string validation");
    }

    [Fact]
    public void MinLength_OnBoolProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinLength = 5)]
        public bool IsActive { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO006");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO006");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Warning);
        diagnostic.GetMessage().Should().Contain("IsActive");
    }

    [Fact]
    public void Pattern_OnDecimalProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(Pattern = ""[0-9]+"")]
        public decimal Price { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO006");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO006");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Warning);
        diagnostic.GetMessage().Should().Contain("Price");
    }

    [Fact]
    public void StringValidation_OnDateTimeProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxLength = 100)]
        public DateTime CreatedAt { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO006");
    }

    [Fact]
    public void AllStringValidations_OnNonStringProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinLength = 5, MaxLength = 50, Pattern = ""^[A-Z]"")]
        public int Code { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO006");
    }

    #endregion

    #region Numeric Validation on Non-Numeric Properties

    [Fact]
    public void MinValue_OnStringProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO007");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Warning);
        diagnostic.GetMessage().Should().Contain("Name");
        diagnostic.GetMessage().Should().Contain("numeric validation");
    }

    [Fact]
    public void MaxValue_OnBoolProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxValue = 100)]
        public bool IsEnabled { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO007");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Warning);
        diagnostic.GetMessage().Should().Contain("IsEnabled");
    }

    [Fact]
    public void RangeValidation_OnStringProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 100)]
        public string Description { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO007");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Warning);
        diagnostic.GetMessage().Should().Contain("Description");
    }

    [Fact]
    public void NumericValidation_OnDateTimeProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 1000)]
        public DateTime CreatedAt { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
    }

    [Fact]
    public void NumericValidation_OnGuidProperty_ReportsWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;
using System;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0)]
        public Guid Id { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
    }

    #endregion

    #region Valid String Validations

    [Fact]
    public void MaxLength_OnStringProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxLength = 50)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void MinLength_OnStringProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinLength = 5)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void Pattern_OnStringProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(Pattern = ""^[A-Z][a-z]+$"")]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void AllStringValidations_OnStringProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinLength = 5, MaxLength = 50, Pattern = ""^[A-Z]"")]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void StringValidations_OnNullableStringProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinLength = 5, MaxLength = 50, Pattern = ""^[A-Z]"")]
        public string? OptionalName { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006");
        generatedSources.Should().NotBeEmpty();
    }

    #endregion

    #region Valid Numeric Validations

    [Fact]
    public void RangeValidation_OnIntProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 100)]
        public int Age { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnDecimalProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0.5, MaxValue = 99.9)]
        public decimal Price { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnLongProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 1000000)]
        public long Count { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnDoubleProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = -100.5, MaxValue = 100.5)]
        public double Temperature { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnFloatProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 100)]
        public float Percentage { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnShortProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 1000)]
        public short Code { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnByteProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 255)]
        public byte Age { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void NumericValidations_OnNullableIntProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 100)]
        public int? OptionalAge { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnUIntProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 1000)]
        public uint Count { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnULongProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 1000000)]
        public ulong LargeCount { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnUShortProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 65535)]
        public ushort Port { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void RangeValidation_OnSByteProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = -128, MaxValue = 127)]
        public sbyte SignedByte { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void NumericValidations_OnNullableDecimalProperty_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0, MaxValue = 9999.99)]
        public decimal? OptionalPrice { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    #endregion

    #region Multiple Validation Errors

    [Fact]
    public void MultipleProperties_WithDifferentValidationErrors_ReportsAllWarnings()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxLength = 50)]
        public int InvalidString { get; set; }

        [DtoProperty(MinValue = 0, MaxValue = 100)]
        public string InvalidNumeric { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().Contain(d => d.Id == "DTO006");
        diagnostics.Should().Contain(d => d.Id == "DTO007");
        diagnostics.Where(d => d.Id == "DTO006" || d.Id == "DTO007").Should().HaveCount(2);
    }

    [Fact]
    public void MultipleProperties_WithSameValidationError_ReportsAllWarnings()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxLength = 50)]
        public int Property1 { get; set; }

        [DtoProperty(MinLength = 10)]
        public decimal Property2 { get; set; }

        [DtoProperty(Pattern = ""test"")]
        public bool Property3 { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Where(d => d.Id == "DTO006").Should().HaveCount(3);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void NoValidationAttributes_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006" || d.Id == "DTO007");
    }

    [Fact]
    public void ValidMixedValidations_DoesNotReportWarning()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxLength = 50)]
        public string Name { get; set; }

        [DtoProperty(MinValue = 0, MaxValue = 100)]
        public int Age { get; set; }

        [DtoProperty(Pattern = ""^[A-Z]"")]
        public string Code { get; set; }

        [DtoProperty(MinValue = 0)]
        public decimal Price { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006" || d.Id == "DTO007");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void OnlyMinValue_WithoutMaxValue_StillValidatesCorrectly()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 0)]
        public string InvalidProperty { get; set; }

        [DtoProperty(MinValue = 0)]
        public int ValidProperty { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO007");
        diagnostic.GetMessage().Should().Contain("InvalidProperty");
    }

    [Fact]
    public void OnlyMaxValue_WithoutMinValue_StillValidatesCorrectly()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxValue = 100)]
        public string InvalidProperty { get; set; }

        [DtoProperty(MaxValue = 100)]
        public int ValidProperty { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO007");
        diagnostic.GetMessage().Should().Contain("InvalidProperty");
    }

    [Fact]
    public void StringAndNumericValidations_OnDifferentProperties_WorksCorrectly()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxLength = 50, MinValue = 0)]
        public string MixedValidation { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO006");
        diagnostics.Should().ContainSingle(d => d.Id == "DTO007");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO007");
        diagnostic.GetMessage().Should().Contain("MixedValidation");
    }

    #endregion

    #region Invalid Range Validations

    [Fact]
    public void MinLength_GreaterThan_MaxLength_ReportsError()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinLength = 10, MaxLength = 5)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO008");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO008");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Error);
        diagnostic.GetMessage().Should().Contain("Name");
        diagnostic.GetMessage().Should().Contain("MinLength");
    }

    [Fact]
    public void MinValue_GreaterThan_MaxValue_ReportsError()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinValue = 10, MaxValue = 5)]
        public int Quantity { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().ContainSingle(d => d.Id == "DTO009");
        var diagnostic = diagnostics.Single(d => d.Id == "DTO009");
        diagnostic.Severity.Should().Be(DiagnosticSeverity.Error);
        diagnostic.GetMessage().Should().Contain("Quantity");
        diagnostic.GetMessage().Should().Contain("MinValue");
    }

    [Fact]
    public void MinLength_Only_DoesNotReportRangeError()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MinLength = 2)]
        public string Name { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO008");
        generatedSources.Should().NotBeEmpty();
    }

    [Fact]
    public void MaxValue_Only_DoesNotReportRangeError()
    {
        // Arrange
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class TestModel
    {
        [DtoProperty(MaxValue = 10)]
        public int Quantity { get; set; }
    }
}";

        // Act
        var (compilation, generatedSources, diagnostics) = SourceGeneratorTestHelper.RunGenerator(source);

        // Assert
        diagnostics.Should().NotContain(d => d.Id == "DTO009");
        generatedSources.Should().NotBeEmpty();
    }

    #endregion
}

