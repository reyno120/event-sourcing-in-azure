using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Infrastructure.Configuration;
using FancyToDo.Projections;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FancyToDo.Functions.ItemAddedEventHandlers;

internal sealed class UpdateProjection(CosmosClient cosmosClient, 
    IOptions<ProjectionOptions> options, ILogger<ItemAddedEvent> logger) 
    : INotificationHandler<ItemAddedEvent>
{
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        // TODO: Move to Decorator?
        logger.LogInformation("Updating Projection for ItemAddedEvent");
        
        var container = cosmosClient
            .GetContainer(options.Value.DatabaseName, options.Value.ContainerName);

        var itemView = new ToDoListItemView()
        {
            Id = @event.ItemId,
            Task = @event.Task,
            Status = @event.Status
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

internal sealed class PublishEvent(ILogger<ItemAddedEvent> logger) : INotificationHandler<ItemAddedEvent>
{
    // TODO: Move to Decorator?
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing ItemAddedEvent");
    }
}