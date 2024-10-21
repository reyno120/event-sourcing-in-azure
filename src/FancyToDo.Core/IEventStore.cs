namespace FancyToDo.Core;

public interface IEventStore
{
    Task<T?> Load<T>(Guid id);
    // Task<IEnumerable<T>> Load<T>();
}