using System.Text.Json;
using System.Text.Json.Serialization;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using SharedKernel;

namespace FancyToDo.Core.ToDoList.DomainEvents;

public record ItemAddedEvent : BaseDomainEvent
{
    public Guid ToDoListId { get; init; }
    public string Item { get; init; }
    
    public ItemAddedEvent(Guid toDoListId, ToDoItem item)
    {
        this.ToDoListId = toDoListId;
        
        // TODO: Best way to handle this? Can't use ToDoItem directly b/c
        // it passes by reference
        // Deserialized example in Cosmos:
        // Item\":\"{\\u0022Task\\u0022:\\u0022test1\\u0022,\\u0022Status\\u0022:\\u0022To Do\\u0022,\\u0022Id
        this.Item = JsonSerializer.Serialize(item);
    }

    [JsonConstructor]
    private ItemAddedEvent(Guid toDoListId, string item)
    {
        this.ToDoListId = toDoListId;
        this.Item = item;
    }
}