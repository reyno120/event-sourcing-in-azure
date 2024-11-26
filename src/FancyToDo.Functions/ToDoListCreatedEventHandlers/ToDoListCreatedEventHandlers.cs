using System.Diagnostics;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Projections;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace FancyToDo.Functions.ToDoListCreatedEventHandlers;


public class CreateProjection(CosmosClient cosmosClient) : INotificationHandler<ToDoListCreatedEvent>
{
    public async Task Handle(ToDoListCreatedEvent @event, CancellationToken cancellationToken)
    {
        // var container = cosmosClient.GetContainer("fancy-db", "ToDoLists");
        // await container.CreateItemAsync(new ToDoListView()
        // {
        //     Id = @event.ToDoListId,
        //     Name = @event.Name
        // }, cancellationToken: cancellationToken)
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