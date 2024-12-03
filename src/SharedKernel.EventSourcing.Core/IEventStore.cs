namespace SharedKernel.EventSourcing.Core;

public interface IEventStore<T>
{
    T Store { get; }
}