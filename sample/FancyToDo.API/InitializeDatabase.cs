using EventSourcing;
using FancyToDo.API.Configuration;
using FancyToDo.Core.ToDoList;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FancyToDo.API;

public static class InitializeDatabase
{
   public static async Task InitializeCosmosDb(this WebApplication app)
   {
      var eventStoreOptionsSnapshot = app.Services.GetRequiredService<IOptionsMonitor<EventStoreOptions>>();
      var eventStoreOptions = eventStoreOptionsSnapshot.Get($"{nameof(ToDoList)}");
      var cosmosClient = app.Services.GetRequiredService<CosmosClient>();
      
      
      // Create database if it doesn't already exist
      var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(eventStoreOptions.DatabaseName);
      
      // Create containers if they don't already exist
      await db.Database.DefineContainer(eventStoreOptions.ContainerName, "/streamId")
         .WithUniqueKey()
         .Path("/version")
         .Attach()
         .CreateIfNotExistsAsync();

      
      var projectionOptions = app.Services.GetRequiredService<IOptions<ProjectionOptions>>();
      await db.Database.CreateContainerIfNotExistsAsync(new ContainerProperties(projectionOptions.Value.ContainerName,
         "/id"));
   } 
}