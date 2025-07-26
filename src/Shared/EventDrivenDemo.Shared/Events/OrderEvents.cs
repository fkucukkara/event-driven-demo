namespace EventDrivenDemo.Shared.Events;

/// <summary>
/// Event published when a new order is created
/// </summary>
public record OrderCreatedEvent : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public int Version { get; init; } = 1;
    
    public Guid OrderId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public List<OrderItem> Items { get; init; } = new();
}

/// <summary>
/// Event published when an order is updated
/// </summary>
public record OrderUpdatedEvent : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public int Version { get; init; } = 1;
    
    public Guid OrderId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public List<OrderItem> Items { get; init; } = new();
    public string Status { get; init; } = string.Empty;
}

/// <summary>
/// Event published when an order is cancelled
/// </summary>
public record OrderCancelledEvent : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public int Version { get; init; } = 1;
    
    public Guid OrderId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
    public List<OrderItem> Items { get; init; } = new();
}

/// <summary>
/// Represents an item in an order
/// </summary>
public record OrderItem
{
    public string ProductId { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
