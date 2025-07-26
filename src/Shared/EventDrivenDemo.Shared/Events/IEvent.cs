namespace EventDrivenDemo.Shared.Events;

/// <summary>
/// Base interface for all domain events
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Unique identifier for the event
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    DateTime OccurredAt { get; }
    
    /// <summary>
    /// Version of the event schema for backward compatibility
    /// </summary>
    int Version { get; }
}
