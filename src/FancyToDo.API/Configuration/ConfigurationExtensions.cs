using FancyToDo.Core;
using FancyToDo.Infrastructure;
using Microsoft.Azure.Cosmos;

namespace FancyToDo.API.Configuration;

public static class ConfigurationExtensions
{
   public static WebApplicationBuilder ConfigureDataStore(this WebApplicationBuilder builder)
   {
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
      builder.Services.AddSingleton<IEventStore, EventStore>();
      builder.Services.AddSingleton<ToDoListReadOnlyRepository>();

      return builder;
   }
}