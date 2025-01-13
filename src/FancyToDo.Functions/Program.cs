using System.Diagnostics;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.EventSourcing.EventStore;
using System.Text.Json;


// Using IHostApplicationBuilder
// https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=ihostapplicationbuilder%2Cwindows#start-up-and-configuration

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();


builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    var clientOptions = new CosmosClientOptions
    {
        SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        }
    }; 
    
    return new CosmosClient(
        builder.Configuration.GetConnectionString("CosmosDBConnectionString"), 
        clientOptions
        );
});


builder.ConfigureEventStore(typeof(Program).Assembly);
builder.Services.Configure<ProjectionOptions>(builder.Configuration.GetSection(ProjectionOptions.Projection));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});


var host = builder.Build();


// TODO: Temporary
// var db = await host.Services.GetRequiredService<CosmosClient>().CreateDatabaseIfNotExistsAsync("fancy-db");
// var eventStream = await db.Database.DefineContainer("ToDoListEventStream", "/streamId")
//     .WithUniqueKey()
//     .Path("/version")
//     .Attach()
//     .CreateIfNotExistsAsync();
//         
// var projection = await db.Database.DefineContainer("ToDoLists", "/id")
//     .CreateIfNotExistsAsync();
//
// /* Seed EventStore */
// var toDoListId = Guid.Parse("381cafbf-9126-43ff-bbd4-eda0eef17e97");
//       
// EventStream stream = new
// (
//     streamId: toDoListId,
//     eventType: typeof(ToDoListCreatedEvent),
//     version: 1,
//     payload: JsonSerializer.Serialize(new ToDoListCreatedEvent(toDoListId, "Fancy ToDo List"))
// );
// await eventStream.Container.UpsertItemAsync(stream);
//       
// await projection.Container.UpsertItemAsync(new
//     { id = toDoListId.ToString(), name = "Fancy ToDo List", items = new List<ToDoItem>() });




await host.RunAsync();

namespace  FancyToDo.Functions
{
    public partial class Program { }
}
