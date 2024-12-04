using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Core.ToDoList.Entities.ToDoItem.DomainEvents;
using SharedKernel.EventSourcing.Core;

namespace FancyToDo.Core.ToDoList;

public partial class ToDoList 
{
    public ToDoList(IEnumerable<BaseDomainEvent> events) : base(events)
    {
    }

    private void Handle(ToDoListCreatedEvent e)
    {
        this.Id = e.ToDoListId;
        this.Name = e.Name;
    }

    private void Handle(ItemAddedEvent e)
    {
        this._items.Add(new ToDoItem(e));
    }

    private void Handle(TaskRenamedEvent e)
    {
        _items
            .Single(s => s.Id == e.TaskId)
            .Mutate(e);
    }
}