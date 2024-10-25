using System.Text.Json;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Core.ToDoList.Entities.ToDoItem.DomainEvents;
using SharedKernel;

namespace FancyToDo.Core.ToDoList;

public partial class ToDoList 
{
    public ToDoList(IEnumerable<BaseDomainEvent> events)
    {
        foreach (var @event in events)
            Mutate(@event);
    }

    private void When(ToDoListCreatedEvent e)
    {
        this.Id = e.ToDoListId;
        this.Name = e.Name;
    }

    private void When(ItemAddedEvent e)
    {
        this._items.Add(JsonSerializer.Deserialize<ToDoItem>(e.Item)!);
    }

    private void When(TaskRenamedEvent e)
    {
        _items
            .Single(s => s.Id == e.TaskId)
            .Mutate(e);
    }
}