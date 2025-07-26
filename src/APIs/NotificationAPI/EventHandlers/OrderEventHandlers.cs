using EventDrivenDemo.Shared.Events;
using EventDrivenDemo.Shared.Messaging;
using NotificationAPI.Services;

namespace NotificationAPI.EventHandlers;

/// <summary>
/// Handles OrderCreated events to send confirmation notifications
/// </summary>
public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly NotificationService _notificationService;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(NotificationService notificationService, ILogger<OrderCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderCreated event for order {OrderId}", @event.OrderId);

        var subject = $"Order Confirmation - Order #{@event.OrderId}";
        var message = $"Thank you for your order!\n\n" +
                     $"Order ID: {@event.OrderId}\n" +
                     $"Total Amount: ${@event.TotalAmount:F2}\n" +
                     $"Items:\n" +
                     string.Join("\n", @event.Items.Select(item => 
                         $"- {item.ProductName} x {item.Quantity} @ ${item.UnitPrice:F2}"));

        await _notificationService.CreateNotificationAsync(
            @event.CustomerEmail,
            subject,
            message,
            "OrderCreated",
            @event.OrderId,
            cancellationToken);

        _logger.LogInformation("Order confirmation notification created for order {OrderId}", @event.OrderId);
    }
}

/// <summary>
/// Handles OrderUpdated events to send status update notifications
/// </summary>
public class OrderUpdatedEventHandler : IEventHandler<OrderUpdatedEvent>
{
    private readonly NotificationService _notificationService;
    private readonly ILogger<OrderUpdatedEventHandler> _logger;

    public OrderUpdatedEventHandler(NotificationService notificationService, ILogger<OrderUpdatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderUpdatedEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderUpdated event for order {OrderId}", @event.OrderId);

        var subject = $"Order Status Update - Order #{@event.OrderId}";
        var message = $"Your order status has been updated.\n\n" +
                     $"Order ID: {@event.OrderId}\n" +
                     $"New Status: {@event.Status}\n" +
                     $"Total Amount: ${@event.TotalAmount:F2}";

        await _notificationService.CreateNotificationAsync(
            @event.CustomerEmail,
            subject,
            message,
            "OrderUpdated",
            @event.OrderId,
            cancellationToken);

        _logger.LogInformation("Order update notification created for order {OrderId}", @event.OrderId);
    }
}

/// <summary>
/// Handles OrderCancelled events to send cancellation notifications
/// </summary>
public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
{
    private readonly NotificationService _notificationService;
    private readonly ILogger<OrderCancelledEventHandler> _logger;

    public OrderCancelledEventHandler(NotificationService notificationService, ILogger<OrderCancelledEventHandler> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing OrderCancelled event for order {OrderId}", @event.OrderId);

        var subject = $"Order Cancellation - Order #{@event.OrderId}";
        var message = $"Your order has been cancelled.\n\n" +
                     $"Order ID: {@event.OrderId}\n" +
                     $"Reason: {@event.Reason}\n" +
                     $"If you have any questions, please contact our support team.";

        await _notificationService.CreateNotificationAsync(
            @event.CustomerEmail,
            subject,
            message,
            "OrderCancelled",
            @event.OrderId,
            cancellationToken);

        _logger.LogInformation("Order cancellation notification created for order {OrderId}", @event.OrderId);
    }
}
