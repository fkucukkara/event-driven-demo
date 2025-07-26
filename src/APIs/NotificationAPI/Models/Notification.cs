namespace NotificationAPI.Models;

/// <summary>
/// Notification entity
/// </summary>
public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty; // "OrderCreated", "OrderUpdated", "OrderCancelled"
    public Guid? OrderId { get; set; }
    public bool IsSent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
}
