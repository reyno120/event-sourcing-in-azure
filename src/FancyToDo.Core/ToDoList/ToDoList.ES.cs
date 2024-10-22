using FancyToDo.Core.ToDoList.DomainEvents;
using SharedKernel;

namespace FancyToDo.Core.ToDoList;

public partial class ToDoList
{
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

    private void When(ItemAddedEvent e)
    {
        this._items.Add(new ToDoItem(e));
    }

    private void When(TaskRenamedEvent e)
    {
        _items
            .Single(s => s.Id == e.TaskId)
            .Mutate(e);
    }
}