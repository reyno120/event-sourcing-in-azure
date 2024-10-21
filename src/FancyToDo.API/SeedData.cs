using FancyToDo.Core.ToDoList;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace FancyToDo.API;

public static class SeedData
{
   public static async Task SeedTestData(this WebApplication app)
   {
      var cosmosClient = app.Services.GetRequiredService<CosmosClient>();
      
      // Create database if it doesn't already exist
      var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(app.Configuration["DatabaseName"]);

      var toDoListId = Guid.NewGuid();
      
      /* Seed EventStore */
      var containerProperties = new ContainerProperties(app.Configuration["EventStoreContainerName"], "/id");
      var container = await db.Database.CreateContainerIfNotExistsAsync(containerProperties);
      await container.Container.CreateItemAsync(new ToDoListCreatedEvent(toDoListId, "Fancy ToDo List"));
      
      
      /* Seed Read Model */
      var readModelContainerProperties = new ContainerProperties(app.Configuration["ReadModelContainerName"], "/id");
      var readModelContainer = await db.Database.CreateContainerIfNotExistsAsync(readModelContainerProperties);

      var queryable = readModelContainer.Container.GetItemLinqQueryable<object>();
      using var linqFeed = queryable.ToFeedIterator();
      var response = await linqFeed.ReadNextAsync();
      
      if(response.Count == 0)
         await readModelContainer.Container.CreateItemAsync(new
            { id = toDoListId.ToString(), name = "Fancy ToDo List", items = new List<ToDoItem>() });
   } 
}