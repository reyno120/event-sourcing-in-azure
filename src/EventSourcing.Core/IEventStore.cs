namespace EventSourcing.Core;

public interface IEventStore<T>
{
    Task Append(T aggregateRoot);
    Task<T?> Load(Guid id);
}