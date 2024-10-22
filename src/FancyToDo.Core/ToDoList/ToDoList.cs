using Ardalis.GuardClauses;
using FancyToDo.Core.ToDoList.DomainEvents;
using SharedKernel;

namespace FancyToDo.Core.ToDoList;

public class ToDoList : Entity, IAggregateRoot
{
    public string Name { get; private set; }

    private readonly List<ToDoItem> _items = [];

    public IReadOnlyList<ToDoItem> Items => _items.AsReadOnly();

    public ToDoList(string name)
    {
        Guard.Against.NullOrEmpty(name);
        Guard.Against.LengthOutOfRange(name, 1, 50);

        Apply(new ToDoListCreatedEvent(this.Id, name));
    }

    public void AddToDo(ToDoItem item)
    {
        if (_items.Any(a => a.Task.Equals(item.Task)))
            throw new InvalidOperationException("Task already exists with that name.");
        
        _items.Add(item);
    }


    #region Sourcing Events

    public ToDoList(IEnumerable<BaseDomainEvent> events)
    {
        foreach (var @event in events)
            Mutate(@event);
    }

    private void Apply(BaseDomainEvent @event)
    {
        this.AddDomainEvent(@event);
        Mutate(@event);
    }

    private void Mutate(BaseDomainEvent @event)
    {
        ((dynamic)this).When((dynamic)@event);
    }

    private void When(ToDoListCreatedEvent e)
    {
        this.Id = e.ToDoListId;
        this.Name = e.Name;
    }
    
    #endregion

}