using System.Diagnostics;
using FancyToDo.Core.ToDoList.DomainEvents;
using MediatR;

namespace FancyToDo.Functions.ToDoListCreatedEventHandlers;

public class CreateProjection : INotificationHandler<ToDoListCreatedEvent>
{
    public async Task Handle(ToDoListCreatedEvent @event, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Create Projection");
    }
}

public class PublishEvent : INotificationHandler<ToDoListCreatedEvent>
{
    public async Task Handle(ToDoListCreatedEvent @event, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Publish Event");
    }
}