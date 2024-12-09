using FancyToDo.Core.ToDoList;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using SharedKernel.EventSourcing.EventStore;

namespace FancyToDo.Infrastructure;

public class ToDoListEventStore(CosmosClient client, EventStoreOptions options, ILogger logger) 
    : EventStore<ToDoList>(client, options, logger)
{
}