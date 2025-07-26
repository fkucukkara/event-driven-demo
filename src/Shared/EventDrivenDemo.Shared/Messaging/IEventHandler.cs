using EventDrivenDemo.Shared.Events;

namespace EventDrivenDemo.Shared.Messaging;

/// <summary>
/// Interface for handling events received from the message broker
/// </summary>
/// <typeparam name="T">Type of event to handle</typeparam>
public interface IEventHandler<in T> where T : class, IEvent
{
    /// <summary>
    /// Handles the received event
    /// </summary>
    /// <param name="event">The event to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}
