using System.Diagnostics;
using System.Text.Json;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
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
        // TODO: Make Configurable
        var container = cosmosClient.GetContainer("fancy-db", "ToDoLists");

        var item = JsonSerializer.Deserialize<ToDoItem>(@event.Item)!;
        var itemView = new ToDoListItemView()
        {
            Id = item.Id,
            Task = item.Task,
            Status = item.Status
        };

        // TODO: Patch vs Replace??
        await container.PatchItemAsync<ToDoListView>(
            id: @event.ToDoListId.ToString(),
            partitionKey: new PartitionKey(@event.ToDoListId.ToString()),
            patchOperations: new[]
            {
                PatchOperation.Add("/items/-", itemView),
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