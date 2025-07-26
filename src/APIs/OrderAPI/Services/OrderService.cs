using EventDrivenDemo.Shared.Events;
using EventDrivenDemo.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs;
using OrderAPI.Models;

namespace OrderAPI.Services;

/// <summary>
/// Service for managing orders and publishing events
/// </summary>
public class OrderService
{
    private readonly OrderDbContext _context;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<OrderService> _logger;

    public OrderService(OrderDbContext context, IEventPublisher eventPublisher, ILogger<OrderService> logger)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            CustomerEmail = request.CustomerEmail,
            Items = request.Items.Select(item => new Models.OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        order.TotalAmount = order.Items.Sum(item => item.TotalPrice);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish OrderCreated event
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(item => new EventDrivenDemo.Shared.Events.OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        await _eventPublisher.PublishAsync(orderCreatedEvent, cancellationToken);
        
        _logger.LogInformation("Created order {OrderId} for customer {CustomerEmail}", order.Id, order.CustomerEmail);

        return order;
    }

    public async Task<Order?> UpdateOrderAsync(Guid orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
            return null;

        order.Status = request.Status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Publish OrderUpdated event
        var orderUpdatedEvent = new OrderUpdatedEvent
        {
            OrderId = order.Id,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            Items = order.Items.Select(item => new EventDrivenDemo.Shared.Events.OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        await _eventPublisher.PublishAsync(orderUpdatedEvent, cancellationToken);
        
        _logger.LogInformation("Updated order {OrderId} status to {Status}", order.Id, order.Status);

        return order;
    }

    public async Task<Order?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
