# GenericDtoSourceGenerator

A powerful C# source generator that automatically generates Data Transfer Objects (DTOs) from your domain models at compile time.

[![.NET](https://img.shields.io/badge/.NET-10.0%2B-512BD4)](https://dotnet.microsoft.com/)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

## Features

- 🚀 **Zero Runtime Overhead**: DTOs are generated at compile time using Roslyn source generators
- 🔄 **Automatic Mapping**: Generates `ToDto()`, `ToEntity()`, and `UpdateFrom()` extension methods
- 📦 **Collection Support**: Built-in support for mapping collections with `ToDtoList()` and `ToEntityList()`
- 🎯 **Flexible Configuration**: Customize DTO names, namespaces, and property behavior
- ✅ **Validation Ready**: Optional `IValidatableObject` implementation
- 🔒 **Type Safe**: Full nullable reference type support
- 📝 **Records Support**: Generate immutable record DTOs

## Installation

### NuGet Package

```bash
dotnet add package BinaryCoffee.GenericDto --version 1.0.0
```

## Quick Start

### 1. Mark your entity with `[GenericDto]`

```csharp
using GenericDto.Core.Attributes;

[GenericDto]
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    
    [DtoIgnore]
    public string InternalCode { get; set; } // Won't appear in DTO
}
```

### 2. Use the generated DTO and mappings

```csharp
// A CustomerDto class is automatically generated in YourNamespace.Dto

var customer = new Customer { Id = 1, Name = "John", Email = "john@example.com" };

// Convert to DTO
var dto = customer.ToDto();

// Convert back to entity
var entity = dto.ToEntity();

// Update existing entity
existingCustomer.UpdateFrom(dto);

// Map collections
var dtos = customers.ToDtoList();
var entities = dtos.ToEntityList();
```

## Configuration Options

### `[GenericDto]` Attribute

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DtoName` | `string?` | `{ClassName}Dto` | Name of the generated DTO |
| `Namespace` | `string?` | `{Namespace}.Dto` | Namespace for the generated DTO |
| `UseRecord` | `bool` | `false` | Generate a record instead of a class |
| `AccessModifier` | `string` | `"public"` | Access modifier for the DTO |
| `GenerateParameterlessConstructor` | `bool` | `true` | Generate parameterless constructor |
| `ImplementIEquatable` | `bool` | `false` | Implement `IEquatable<T>` |
| `ImplementIValidatableObject` | `bool` | `false` | Implement `IValidatableObject` |
| `AdditionalUsings` | `string[]` | `[]` | Additional using statements |
| `IncludeInheritedProperties` | `bool` | `true` | Include properties from base classes |
| `GenerateMappers` | `bool` | `true` | Generate mapping extension methods |

### `[DtoProperty]` Attribute

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Name` | `string?` | Property name | Custom name for the DTO property |
| `Ignore` | `bool` | `false` | Exclude property from DTO |
| `Type` | `Type?` | Original type | Custom type for the property |
| `DefaultValue` | `string?` | None | Default value expression |
| `ForceNullable` | `bool?` | Inherited | Force nullable/non-nullable |
| `Order` | `int` | `0` | Property order in DTO |
| `Description` | `string?` | Auto | Custom XML documentation |
| `MaxLength` | `int` | `-1` | Maximum length validation |
| `MinLength` | `int` | `-1` | Minimum length validation |
| `Pattern` | `string?` | None | Regex pattern validation |
| `MinValue` | `double` | `double.MinValue` | Minimum value for range |
| `MaxValue` | `double` | `double.MaxValue` | Maximum value for range |

### `[DtoIgnore]` Attribute

Shorthand for `[DtoProperty(Ignore = true)]` - excludes the property from the generated DTO.

### `[DtoValidation]` Attribute

| Property | Type | Description |
|----------|------|-------------|
| `Required` | `bool` | Adds `[Required]` validation |
| `EmailAddress` | `bool` | Adds `[EmailAddress]` validation |
| `Phone` | `bool` | Adds `[Phone]` validation |
| `Url` | `bool` | Adds `[Url]` validation |
| `CreditCard` | `bool` | Adds `[CreditCard]` validation |
| `ErrorMessage` | `string?` | Custom error message |
| `CompareProperty` | `string?` | Property to compare against |

## Advanced Examples

### Using Records

```csharp
[GenericDto(UseRecord = true)]
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Generates: public record ProductDto { ... }
```

### Custom Namespace and Name

```csharp
[GenericDto(
    DtoName = "OrderResponse",
    Namespace = "MyApi.Responses")]
public class Order
{
    public Guid OrderId { get; set; }
    public decimal TotalAmount { get; set; }
}

// Generates: MyApi.Responses.OrderResponse
```

### With Validation

```csharp
[GenericDto(ImplementIValidatableObject = true)]
public class User
{
    public string Username { get; set; }
    
    [DtoProperty(MaxLength = 100, MinLength = 5)]
    public string Password { get; set; }
    
    [DtoValidation(EmailAddress = true, Required = true)]
    public string Email { get; set; }
}
```

### Property Customization

```csharp
[GenericDto]
public class Employee
{
    public int Id { get; set; }
    
    [DtoProperty(Name = "FullName")]
    public string Name { get; set; }
    
    [DtoProperty(DefaultValue = "DateTime.UtcNow")]
    public DateTime HireDate { get; set; }
    
    [DtoIgnore]
    public decimal Salary { get; set; } // Sensitive data excluded
}
```

## Generated Code

For a class like:

```csharp
[GenericDto]
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

The generator produces:

```csharp
// CustomerDto.g.cs
namespace YourNamespace.Dto
{
    public partial class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public CustomerDto() { }
        
        public CustomerDto(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

// CustomerDtoMapper.g.cs
namespace YourNamespace.Dto
{
    public static class CustomerDtoMapperExtensions
    {
        public static CustomerDto ToDto(this Customer source) { ... }
        public static Customer ToEntity(this CustomerDto dto) { ... }
        public static List<CustomerDto> ToDtoList(this IEnumerable<Customer> source) { ... }
        public static Customer UpdateFrom(this Customer entity, CustomerDto dto) { ... }
    }
}
```

## Requirements

- .NET 10.0+ (for the example project)
- .NET Standard 2.0 compatible (for the analyzers and core attributes)
- Visual Studio 2022 or later / Rider / VS Code with C# extension

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](LICENSE.txt) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Acknowledgments

- Built with [Roslyn](https://github.com/dotnet/roslyn) Source Generators
- Inspired by modern DTO mapping patterns
