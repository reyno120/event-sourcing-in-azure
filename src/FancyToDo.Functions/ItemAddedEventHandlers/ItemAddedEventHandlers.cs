﻿using System.Diagnostics;
using FancyToDo.Core.ToDoList.DomainEvents;
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

public class PublishEvent : INotificationHandler<ItemAddedEvent>
{
    public async Task Handle(ItemAddedEvent @event, CancellationToken cancellationToken)
    {
        Debug.WriteLine("Publish Event");
    }
}