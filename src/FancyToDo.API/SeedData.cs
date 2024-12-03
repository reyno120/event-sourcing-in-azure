using System.Text.Json;
using FancyToDo.Core.ToDoList;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using SharedKernel.EventSourcing.EventStore;

namespace FancyToDo.API;

public static class SeedData
{
   public static async Task SeedTestData(this WebApplication app)
   {
      var eventStoreOptionsSnapshot = app.Services.GetRequiredService<IOptionsMonitor<EventStoreOptions>>();
      var eventStoreOptions = eventStoreOptionsSnapshot.Get($"{nameof(ToDoList)}EventStore");
      
      var projectionOptions = app.Services.GetRequiredService<IOptions<ProjectionOptions>>();
      
      var cosmosClient = app.Services.GetRequiredService<CosmosClient>();
      
      // Create database if it doesn't already exist
      var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(eventStoreOptions.DatabaseName);
      
      // Create container if it doesn't already exist
      var container = await db.Database.DefineContainer(eventStoreOptions.ContainerName, "/streamId")
         .WithUniqueKey()
         .Path("/version")
         .Attach()
         .CreateIfNotExistsAsync();

      
      /* Seed EventStore */
      var toDoListId = Guid.Parse("381cafbf-9126-43ff-bbd4-eda0eef17e97");
      
      EventStream stream = new
      (
         streamId: toDoListId,
         timeStamp: DateTimeOffset.UtcNow,
         eventType: typeof(ToDoListCreatedEvent),
         version: 1,
         payload: JsonSerializer.Serialize(new ToDoListCreatedEvent(toDoListId, "Fancy ToDo List"))
      );
      await container.Container.UpsertItemAsync(stream);
      
      
      /* Seed Read Model */
      var readModelContainerProperties = new ContainerProperties(projectionOptions.Value.ContainerName, "/id");
      var readModelContainer = await db.Database.CreateContainerIfNotExistsAsync(readModelContainerProperties);
      await readModelContainer.Container.UpsertItemAsync(new
         { id = toDoListId.ToString(), name = "Fancy ToDo List", items = new List<ToDoItem>() });
   } 
}