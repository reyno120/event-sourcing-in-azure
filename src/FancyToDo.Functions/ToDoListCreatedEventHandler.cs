using System.Diagnostics;
using FancyToDo.Core.ToDoList;
using Microsoft.Azure.Cosmos;

namespace FancyToDo.Functions;

public class ToDoListCreatedEventHandler() : IEventHandler
// public class ToDoListCreatedEventHandler(CosmosClient cosmosClient) : IEventHandler
{
    public async Task Handle(object e)
    {
        var createdEvent = (ToDoListCreatedEvent)e;
        Debug.WriteLine(createdEvent.ToString());
    }
}