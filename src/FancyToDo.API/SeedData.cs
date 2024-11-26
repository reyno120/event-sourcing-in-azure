﻿using System.Text.Json;
using FancyToDo.Core.ToDoList;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using Microsoft.Azure.Cosmos;
using SharedKernel;

namespace FancyToDo.API;

public static class SeedData
{
   public static async Task SeedTestData(this WebApplication app)
   {
      // TODO: Make Configurable
      const string databaseName = "fancy-db";
      const string eventStoreContainerName = "ToDoListEventStream";
      const string readModelContainerName = "ToDoLists";
      
      var cosmosClient = app.Services.GetRequiredService<CosmosClient>();
      
      // Create database if it doesn't already exist
      var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);
      
      // Create container if it doesn't already exist
      var container = await db.Database.DefineContainer(eventStoreContainerName, "/streamId")
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
      var readModelContainerProperties = new ContainerProperties(readModelContainerName, "/id");
      var readModelContainer = await db.Database.CreateContainerIfNotExistsAsync(readModelContainerProperties);
      await readModelContainer.Container.UpsertItemAsync(new
         { id = toDoListId.ToString(), name = "Fancy ToDo List", items = new List<ToDoItem>() });
   } 
}