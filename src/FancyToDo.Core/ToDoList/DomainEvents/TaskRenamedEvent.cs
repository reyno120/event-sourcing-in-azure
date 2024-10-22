using SharedKernel;

namespace FancyToDo.Core.ToDoList.DomainEvents;

public record TaskRenamedEvent(Guid TaskId, string Name) : BaseDomainEvent;