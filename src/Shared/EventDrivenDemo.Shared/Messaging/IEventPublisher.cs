using EventDrivenDemo.Shared.Events;

namespace EventDrivenDemo.Shared.Messaging;

/// <summary>
/// Interface for publishing events to the message broker
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to the message broker
    /// </summary>
    /// <typeparam name="T">Type of event to publish</typeparam>
    /// <param name="event">The event to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class, IEvent;
}
