using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using SharedKernel;

namespace FancyToDo.Core.ToDoList.DomainEvents;

public record ItemAddedEvent(Guid ToDoListId, ToDoItem Item): BaseDomainEvent;