using FancyToDo.Core;
using FancyToDo.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace FancyToDo.API.Configuration;

public static class ConfigurationExtensions
{
   public static WebApplicationBuilder ConfigureDataStore(this WebApplicationBuilder builder)
   {
      var databaseName = builder.Configuration["DatabaseName"]!;
      
      builder.AddAzureCosmosClient(
         "local-fancy-cosmos",
         null,
         clientOptions =>
         {
            clientOptions.SerializerOptions = new()
            {
               PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            };
         });
      
      var cosmosClient = builder.Services.BuildServiceProvider().GetService<CosmosClient>()!;
      builder.Services.AddSingleton<IEventStore>(new EventStore(cosmosClient, databaseName, builder.Configuration["EventStoreContainerName"]!));
      builder.Services.AddSingleton(new ToDoListReadRepository(cosmosClient, databaseName,
         builder.Configuration["ReadModelContainerName"]!));

      return builder;
   }
}