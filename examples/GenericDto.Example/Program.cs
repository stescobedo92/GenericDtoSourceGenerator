using GenericDto.Example.Models;
using GenericDto.Example.Models.Dto;

namespace GenericDto.Example;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== GenericDto Source Generator Example ===\n");

        // Example 1: Using generated DTO with Customer
        DemonstrateCustomerDto();

        // Example 2: Using generated DTO with Product (record)
        DemonstrateProductDto();

        // Example 3: Using mapping extensions
        DemonstrateMappingExtensions();

        Console.WriteLine("\n=== Demo Complete ===");
    }

    static void DemonstrateCustomerDto()
    {
        Console.WriteLine("1. Customer DTO Example:");
        Console.WriteLine("------------------------");

        // Create a customer entity
        var customer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            InternalCode = "INTERNAL-001", // This won't appear in DTO
            PhoneNumber = "+1-555-0123"
        };

        // Convert to DTO using generated extension method
        var customerDto = customer.ToDto();

        Console.WriteLine($"  Original Customer: Id={customer.Id}, Name={customer.Name}");
        Console.WriteLine($"  Customer DTO: {customerDto}");
        Console.WriteLine();
    }

    static void DemonstrateProductDto()
    {
        Console.WriteLine("2. Product DTO Example (Record):");
        Console.WriteLine("---------------------------------");

        var product = new Product
        {
            Id = 100,
            Name = "Wireless Mouse",
            Price = 29.99m,
            Stock = 50,
            Sku = "WM-001" // This won't appear in DTO
        };

        var productDto = product.ToDto();

        Console.WriteLine($"  Original Product: Id={product.Id}, Name={product.Name}, Price=${product.Price}");
        Console.WriteLine($"  Product DTO: {productDto}");
        Console.WriteLine();
    }

    static void DemonstrateMappingExtensions()
    {
        Console.WriteLine("3. Mapping Extensions Example:");
        Console.WriteLine("------------------------------");

        // Create a list of customers
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Alice", Email = "alice@example.com", IsActive = true },
            new() { Id = 2, Name = "Bob", Email = "bob@example.com", IsActive = true },
            new() { Id = 3, Name = "Charlie", Email = "charlie@example.com", IsActive = false }
        };

        // Convert entire list to DTOs
        var customerDtos = customers.ToDtoList();

        Console.WriteLine($"  Converted {customers.Count} customers to DTOs:");
        foreach (var dto in customerDtos)
        {
            Console.WriteLine($"    - {dto.FullName} ({dto.Email})");
        }
        Console.WriteLine();

        // Update an existing entity from a DTO
        var updateDto = new CustomerDto
        {
            Id = 1,
            FullName = "Alice Updated",
            Email = "alice.updated@example.com",
            IsActive = true
        };

        var existingCustomer = customers.First(c => c.Id == 1);
        existingCustomer.UpdateFrom(updateDto);

        Console.WriteLine($"  Updated customer: {existingCustomer.Name} - {existingCustomer.Email}");
    }
}
