using SharedKernel.EventSourcing.Core;

namespace FancyToDo.Core.ToDoList.DomainEvents;

public record ItemAddedEvent(Guid ToDoListId, Guid ItemId, string Task, string Status) : BaseDomainEvent;