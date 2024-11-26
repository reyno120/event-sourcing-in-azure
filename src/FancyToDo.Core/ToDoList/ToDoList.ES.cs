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
        { 
            _version++;
            Mutate(@event);
        }
    }

    private void Handle(ToDoListCreatedEvent e)
    {
        this.Id = e.ToDoListId;
        this.Name = e.Name;
    }

    private void Handle(ItemAddedEvent e)
    {
        this._items.Add(JsonSerializer.Deserialize<ToDoItem>(e.Item)!);
    }

    private void Handle(TaskRenamedEvent e)
    {
        _items
            .Single(s => s.Id == e.TaskId)
            .Mutate(e);
    }
}