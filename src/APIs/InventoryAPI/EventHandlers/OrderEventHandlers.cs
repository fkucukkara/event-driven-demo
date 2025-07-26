using EventDrivenDemo.Shared.Events;
using EventDrivenDemo.Shared.Messaging;
using InventoryAPI.Services;

namespace InventoryAPI.EventHandlers;

/// <summary>
/// Handles OrderCreated events to reserve inventory
/// </summary>
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly InventoryService _inventoryService;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(InventoryService inventoryService, ILogger<OrderCreatedEventHandler> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderCreated event for order {OrderId}", @event.OrderId);

        var items = @event.Items.Select(item => (item.ProductId, item.Quantity)).ToList();
        
        var success = await _inventoryService.ReserveInventoryAsync(@event.OrderId, items, cancellationToken);
        
        if (success)
        {
            _logger.LogInformation("Successfully reserved inventory for order {OrderId}", @event.OrderId);
        }
        else
        {
            _logger.LogWarning("Failed to reserve inventory for order {OrderId}", @event.OrderId);
        }
    }
}

/// <summary>
/// Handles OrderCancelled events to release reserved inventory
/// </summary>
public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
{
    private readonly InventoryService _inventoryService;
    private readonly ILogger<OrderCancelledEventHandler> _logger;

    public OrderCancelledEventHandler(InventoryService inventoryService, ILogger<OrderCancelledEventHandler> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderCancelled event for order {OrderId}", @event.OrderId);

        var items = @event.Items.Select(item => (item.ProductId, item.Quantity)).ToList();
        
        await _inventoryService.ReleaseInventoryAsync(@event.OrderId, items, cancellationToken);
        
        _logger.LogInformation("Released inventory for cancelled order {OrderId}", @event.OrderId);
    }
}
