using Ardalis.GuardClauses;
using SharedKernel;

namespace FancyToDo.Core;

public class ToDoList : Entity, IAggregateRoot
{
    public string Name { get; private set; }

    private readonly List<ToDoItem> _items = [];

    public IReadOnlyList<ToDoItem> Items => _items.AsReadOnly();

    public ToDoList(string name)
    {
        Guard.Against.NullOrEmpty(name);
        Guard.Against.LengthOutOfRange(name, 1, 50);

        this.Name = name;
        this.Id = Guid.NewGuid();
        
        this.AddDomainEvent(new ToDoListCreatedEvent(this.Id, this.Name));
    }

    public void AddToDo(ToDoItem item)
    {
        if (_items.Any(a => a.Task.Equals(item.Task)))
            throw new InvalidOperationException("Task already exists with that name.");
        
        _items.Add(item);
    }


    #region Sourcing Events

    public ToDoList(IEnumerable<IEvent> events)
    {
        foreach (var @event in events)
            Mutate(@event);
    }

    private void Mutate(IEvent @event)
    {
        ((dynamic)this).When((dynamic)@event);
    }

    private void When(ToDoListCreatedEvent e)
    {
        this.Id = e.Id;
        this.Name = e.Name;
    }
    
    #endregion

}

public record ToDoListCreatedEvent(Guid Id, string Name) : IEvent;