using GenericDto.Core.Attributes;

namespace GenericDto.Example.Models;

/// <summary>
/// Example entity class demonstrating DTO generation.
/// </summary>
[GenericDto(
    DtoName = "CustomerDto",
    UseRecord = false,
    ImplementIEquatable = true,
    GenerateMappers = true)]
public class Customer
{
    public int Id { get; set; }

    [DtoProperty(Name = "FullName", MaxLength = 100)]
    public string Name { get; set; } = string.Empty;

    [DtoProperty(Description = "Customer email address")]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [DtoIgnore]
    public string InternalCode { get; set; } = string.Empty;

    [DtoProperty(ForceNullable = true)]
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Example using record generation.
/// </summary>
[GenericDto(UseRecord = true)]
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    [DtoIgnore]
    public string Sku { get; set; } = string.Empty;
}

/// <summary>
/// Example with custom namespace.
/// </summary>
[GenericDto(
    Namespace = "GenericDto.Example.Api.Responses",
    DtoName = "OrderResponse",
    ImplementIValidatableObject = true)]
public class Order
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
}

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
