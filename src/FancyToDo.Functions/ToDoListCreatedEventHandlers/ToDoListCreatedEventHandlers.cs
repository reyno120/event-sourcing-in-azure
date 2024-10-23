using System.Diagnostics;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Projections;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace FancyToDo.Functions.ToDoListCreatedEventHandlers;

// TODO: Don't inject IConfiguration
// https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows
public class CreateProjection(CosmosClient cosmosClient) : INotificationHandler<ToDoListCreatedEvent>
{
    public async Task Handle(ToDoListCreatedEvent @event, CancellationToken cancellationToken)
    {
        var container = cosmosClient.GetContainer("fancy-db", "ToDoLists");
        await container.CreateItemAsync(new ToDoListView()
        {
            Id = @event.ToDoListId,
            Name = @event.Name
        }, cancellationToken: cancellationToken);
    }
}

public class PublishEvent : INotificationHandler<ToDoListCreatedEvent>
{
    public async Task Handle(ToDoListCreatedEvent @event, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Publish Event");
    }
}