namespace EventSourcing.Core;

/// <summary>
/// Operations for performing reads/writes to an event store.
/// </summary>
/// <typeparam name="T">The type of object for which events are being tracked. Must be the root of an aggregate</typeparam>
public interface IEventStore<T> 
    where T : AggregateRoot
{
    /// <summary>
    /// Append events to the aggregate root's event stream. The aggregate root should contain the events that need to
    /// be appended to the event stream.
    /// </summary>
    /// <param name="aggregateRoot">The root of the aggregate.</param>
    Task Append(T aggregateRoot);
    
    /// <summary>
    /// Rehydrates an aggregate with a given Id. 
    /// </summary>
    /// <param name="id">The Id of the aggregate root.</param>
    /// <returns>The rehydrated aggregate</returns>
    /// <exception cref="AggregateNotFoundException">Thrown if no aggregate is found, or it does not contain any events.</exception>
    Task<T> Load(Guid id);
    
    /// <summary>
    /// Similar to "Load", except if no aggregate is found a null value is returned.
    /// </summary>
    /// <param name="id">The Id of the aggregate root.</param>
    /// <returns>The rehydrated aggregate</returns>
    Task<T?> TryLoad(Guid id);
}