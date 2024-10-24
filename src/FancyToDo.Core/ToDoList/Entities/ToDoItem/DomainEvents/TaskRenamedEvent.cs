using SharedKernel;

namespace FancyToDo.Core.ToDoList.Entities.ToDoItem.DomainEvents;

public record TaskRenamedEvent(Guid TaskId, string Name) : BaseDomainEvent;