using System.Text.Json.Serialization;
using SharedKernel;

namespace FancyToDo.Core.ToDoList;

public record ToDoListCreatedEvent(Guid Id, string Name) : IEvent;