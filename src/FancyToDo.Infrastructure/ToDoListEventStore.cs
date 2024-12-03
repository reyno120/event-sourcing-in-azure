using FancyToDo.Core.ToDoList;
using Microsoft.Azure.Cosmos;
using SharedKernel.EventSourcing.EventStore;

namespace FancyToDo.Infrastructure;

public class ToDoListEventStore(CosmosClient client, EventStoreOptions options) 
    : EventStore<ToDoList>(client, options)
{
}