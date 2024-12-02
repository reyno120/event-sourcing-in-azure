using FancyToDo.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.EventSourcing.EventStore;


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


builder.ConfigureEventStore();
builder.Services.Configure<ProjectionOptions>(builder.Configuration.GetSection(ProjectionOptions.Projection));


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});


var host = builder.Build();

await host.RunAsync();

