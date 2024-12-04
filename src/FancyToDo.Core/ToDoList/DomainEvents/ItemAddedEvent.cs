using System.Text.Json;
using System.Text.Json.Serialization;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using SharedKernel.EventSourcing.Core;

namespace FancyToDo.Core.ToDoList.DomainEvents;

public record ItemAddedEvent : BaseDomainEvent
{
    public Guid ToDoListId { get; init; }
    
    [JsonConverter(typeof(RawJsonStringConverter))]
    public string Item { get; init; }
    
    public ItemAddedEvent(Guid toDoListId, ToDoItem item)
    {
        this.ToDoListId = toDoListId;
        
        // Can't use ToDoItem directly b/c it passes by reference
        this.Item = JsonSerializer.Serialize(item);
    }

    [JsonConstructor]
    private ItemAddedEvent(Guid toDoListId, string item)
    {
        this.ToDoListId = toDoListId;
        this.Item = item;
    }
}