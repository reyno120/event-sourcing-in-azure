using SharedKernel;

namespace FancyToDo.Core.ToDoList.DomainEvents;

public record ItemAddedEvent(Guid ToDoListId, Guid Id, string Task, string Status): BaseDomainEvent;