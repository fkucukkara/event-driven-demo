using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs;

/// <summary>
/// Request DTO for creating a new order
/// </summary>
public class CreateOrderRequest
{
    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [Required]
    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

/// <summary>
/// Request DTO for creating an order item
/// </summary>
public class CreateOrderItemRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;
    
    [Required]
    public string ProductName { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Request DTO for updating an order
/// </summary>
public class UpdateOrderRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
