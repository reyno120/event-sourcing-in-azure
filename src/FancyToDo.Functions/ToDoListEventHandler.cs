using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FancyToDo.Functions
{
    public class ToDoListEventHandler(ILoggerFactory loggerFactory, IMediator mediator)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<ToDoListEventHandler>();

        // Local settings and publishing to Azure
        // https://learn.microsoft.com/en-us/azure/azure-functions/functions-develop-local#local-settings-file
        
        // Accessing environment variables
        // https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-class-library?tabs=v4%2Ccmd#environment-variables
        [Function("ToDoListEventStreamHandler")]  
        public async Task Run([CosmosDBTrigger(
            databaseName: "%DatabaseName%",
            containerName: "%ContainerName%",
            Connection = "CosmosDBConnectionString",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<EventStream> input)
        {
            // Publish Events using MediatR. Here, we can:
            // 1. Update Read Model & Create Projections/Materialized Views
            // 2. Publish Event to Message Bus to Notify Other Services
            // 3. Handle "Side-Effects". For Example, When Another Aggregate Needs to Respond to an Event
            foreach (var stream in input)
                await mediator.Publish(stream.Deserialize());
        }
    }
}
