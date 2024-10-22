using SharedKernel;

namespace FancyToDo.Core;

public interface IEventStore
{
    Task Append<T>(T aggregate) where T : AggregateRoot;
    Task<T?> Load<T>(Guid id) where T : AggregateRoot;
}