using GenericDto.Tests.Helpers;

namespace GenericDto.Tests;

public class UsabilityFeatureTests
{
    [Fact]
    public void Should_Not_Generate_Mappers_When_Disabled()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(GenerateMappers = false)]
    public class Customer
    {
        public int Id { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs").Should().NotBeNull();
        SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs").Should().BeNull();
    }

    [Fact]
    public void Should_Respect_Inherited_Property_And_Documentation_Options()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    public class BaseCustomer
    {
        public int Id { get; set; }
    }

    [GenericDto(IncludeInheritedProperties = false, GenerateDocumentation = false)]
    public class Customer : BaseCustomer
    {
        public string Name { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("Name");
        dtoSource.Should().NotContain("Id");
        dtoSource.Should().NotContain("/// <summary>");
    }

    [Fact]
    public void Should_Apply_DtoValidation_Attributes()
    {
        var source = @"
using GenericDto.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    public static class UserValidators
    {
        public static ValidationResult? IsValidEmail(object? value, ValidationContext context) => ValidationResult.Success;
    }

    [GenericDto]
    public class User
    {
        [DtoValidation(Required = true, EmailAddress = true, ErrorMessage = ""Email required"")]
        public string? Email { get; set; }

        [DtoValidation(CompareProperty = ""Password"")]
        public string ConfirmPassword { get; set; }

        [DtoValidation(CustomValidationType = typeof(UserValidators), CustomValidationMethod = ""IsValidEmail"")]
        public string SecondaryEmail { get; set; }

        public string Password { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "UserDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.Required]");
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = \"Email required\")]");
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.Compare(\"Password\")]");
        dtoSource.Should().Contain("[global::System.ComponentModel.DataAnnotations.CustomValidation(typeof(global::TestNamespace.UserValidators), nameof(global::TestNamespace.UserValidators.IsValidEmail))]");
    }

    [Fact]
    public void Should_Order_Properties_By_DtoProperty_Order()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto]
    public class Customer
    {
        [DtoProperty(Order = 2)]
        public string Last { get; set; }

        [DtoProperty(Order = 1)]
        public string First { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource!.IndexOf("First", StringComparison.Ordinal).Should().BeLessThan(dtoSource.IndexOf("Last", StringComparison.Ordinal));
    }

    [Fact]
    public void Should_Generate_Custom_Mapper_Shape_And_Optimized_Collections()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(MapperNamespace = ""TestNamespace.Mapping"", MapperClassName = ""CustomerMaps"", GenerateToEntity = false, GenerateUpdateFrom = false)]
    public class Customer
    {
        public int Id { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("namespace TestNamespace.Mapping");
        mapperSource.Should().Contain("public static class CustomerMaps");
        mapperSource.Should().Contain("ICollection<TestNamespace.Customer>");
        mapperSource.Should().Contain("foreach (var item in source)");
        mapperSource.Should().NotContain("ToEntity(this");
        mapperSource.Should().NotContain("UpdateFrom(this");
    }

    [Fact]
    public void Should_Generate_Multiple_Dtos_For_One_Source_Type()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(DtoName = ""CustomerCreateDto"", GenerateToEntity = false)]
    [GenericDto(DtoName = ""CustomerResponseDto"")]
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerCreateDto.g.cs").Should().NotBeNull();
        SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerResponseDto.g.cs").Should().NotBeNull();
    }

    [Fact]
    public void Should_Emit_Json_Init_Required_And_Sensitive_ToString_Options()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    [GenericDto(UseInitOnlyProperties = true, UseRequiredMembers = true)]
    public class Customer
    {
        [DtoProperty(JsonPropertyName = ""full_name"")]
        public string Name { get; set; }

        [DtoProperty(Sensitive = true, JsonIgnore = true)]
        public string Token { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("[global::System.Text.Json.Serialization.JsonPropertyName(\"full_name\")]");
        dtoSource.Should().Contain("[global::System.Text.Json.Serialization.JsonIgnore]");
        dtoSource.Should().Contain("public required string Name { get; init; }");
        dtoSource.Should().NotContain("Token = {Token}");
    }

    [Fact]
    public void Should_Map_With_Custom_Source_Target_And_Converter()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    public static class NameConverter
    {
        public static string Convert(string value) => value;
    }

    [GenericDto]
    public class Customer
    {
        public string FirstName { get; set; }

        [DtoProperty(Name = ""Name"", MapFrom = ""FirstName"", MapTo = ""FirstName"", ConverterType = typeof(NameConverter), ConverterMethod = ""Convert"")]
        public string DisplayName { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("Name = global::TestNamespace.NameConverter.Convert(source.FirstName),");
        mapperSource.Should().Contain("FirstName = global::TestNamespace.NameConverter.Convert(dto.Name),");
    }

    [Fact]
    public void Should_Map_Flattened_Source_Path_To_Dto()
    {
        var source = @"
using GenericDto.Core.Attributes;

namespace TestNamespace
{
    public class Address
    {
        public string City { get; set; }
    }

    [GenericDto]
    public class Customer
    {
        [DtoProperty(Name = ""AddressCity"", Type = typeof(string), MapFrom = ""Address.City"", Flatten = true)]
        public Address Address { get; set; }
    }
}";

        var (compilation, generatedSources, _) = SourceGeneratorTestHelper.RunGenerator(source);

        SourceGeneratorTestHelper.VerifyNoCompilationErrors(compilation);
        var dtoSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "CustomerDto.g.cs");
        var mapperSource = SourceGeneratorTestHelper.GetGeneratedSource(generatedSources, "Mapper.g.cs");

        dtoSource.Should().NotBeNull();
        dtoSource.Should().Contain("public string AddressCity { get; set; }");
        mapperSource.Should().NotBeNull();
        mapperSource.Should().Contain("AddressCity = source.Address.City,");
        mapperSource.Should().NotContain("Address = dto.AddressCity");
    }
}
