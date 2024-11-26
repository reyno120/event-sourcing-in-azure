using System.Diagnostics;
using System.Text.Json;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Infrastructure.Configuration;
using FancyToDo.Projections;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FancyToDo.Functions.ItemAddedEventHandlers;

public class UpdateProjection(CosmosClient cosmosClient, IOptions<ProjectionOptions> options) 
    : INotificationHandler<ItemAddedEvent>
{
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        // TODO: Make Configurable
        var container = cosmosClient.GetContainer(options.Value.DatabaseName, options.Value.ContainerName);

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