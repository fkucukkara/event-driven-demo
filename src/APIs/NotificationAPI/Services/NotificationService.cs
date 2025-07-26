using Microsoft.EntityFrameworkCore;
using NotificationAPI.Data;
using NotificationAPI.Models;

namespace NotificationAPI.Services;

/// <summary>
/// Service for managing notifications
/// </summary>
public class NotificationService
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(NotificationDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task CreateNotificationAsync(
        string recipientEmail,
        string subject,
        string message,
        string notificationType,
        Guid? orderId = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            RecipientEmail = recipientEmail,
            Subject = subject,
            Message = message,
            NotificationType = notificationType,
            OrderId = orderId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        // Simulate sending notification (in real app, this would integrate with email service)
        await SimulateSendNotificationAsync(notification, cancellationToken);
    }

    private async Task SimulateSendNotificationAsync(Notification notification, CancellationToken cancellationToken)
    {
        // Simulate email sending delay
        await Task.Delay(100, cancellationToken);

        notification.IsSent = true;
        notification.SentAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Notification sent to {Email}: {Subject}", 
            notification.RecipientEmail, notification.Subject);
    }

    public async Task<List<Notification>> GetNotificationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Notification>> GetNotificationsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.RecipientEmail == email)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
