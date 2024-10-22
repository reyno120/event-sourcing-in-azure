using System.Diagnostics;
using FancyToDo.Core.ToDoList.DomainEvents;
using MediatR;

namespace FancyToDo.Functions.ItemAddedEventHandlers;

public class UpdateProjection : INotificationHandler<ItemAddedEvent>
{
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Create Projection");
    }
}

public class PublishEvent : INotificationHandler<ItemAddedEvent>
{
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Publish Event");
    }
}