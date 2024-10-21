using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FancyToDo.Functions
{
    public class ToDoListEventHandler
    {
        private readonly ILogger _logger;
        // private readonly CosmosClient _cosmosClient;

        // public ToDoListEventHandler(ILoggerFactory loggerFactory, CosmosClient cosmosClient)
        public ToDoListEventHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ToDoListEventHandler>();
            // _cosmosClient = cosmosClient;
        }

        [Function("ToDoListEventHandler")]  // TODO: change this name
        public async Task Run([CosmosDBTrigger(
            databaseName: "fancy-db",
            containerName: "ToDoListEventStream",
            Connection = "fancy-db",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<EventStream> input)
        {
            foreach (var stream in input)
            {
                var handler = (IEventHandler)Activator.CreateInstance(
                    Type.GetType($"{stream.EventType.ToString()}Handler"),
                    // new object[] { _cosmosClient }
                    new object[] { }
                );

                if (handler is null)
                    return;

                await handler.Handle(stream.Deserialize());
            }
        }
    }
}
