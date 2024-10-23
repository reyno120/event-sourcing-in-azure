using System.Diagnostics;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Projections;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace FancyToDo.Functions.ItemAddedEventHandlers;

// TODO: Don't inject IConfiguration
// https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows
public class UpdateProjection(CosmosClient cosmosClient) : INotificationHandler<ItemAddedEvent>
{
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        var container = cosmosClient.GetContainer("fancy-db", "ToDoLists");

        var newItem = new ToDoListView.ToDoListItemView()
        {
            Id = @event.Id,
            Task = @event.Task,
            Status = @event.Status
        };
        
        // TODO: Patch vs Replace??
        await container.PatchItemAsync<ToDoListView>(
            id: @event.ToDoListId.ToString(),
            partitionKey: new PartitionKey(@event.ToDoListId.ToString()),
            patchOperations: new[]
            {
                PatchOperation.Add("/items/-", newItem),
            }, cancellationToken: cancellationToken);
    }
}

public class PublishEvent : INotificationHandler<ItemAddedEvent>
{
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Publish Event");
    }
}