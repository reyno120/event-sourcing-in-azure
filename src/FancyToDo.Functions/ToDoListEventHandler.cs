using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FancyToDo.Functions
{
    public class ToDoListEventHandler(ILoggerFactory loggerFactory, IMediator mediator)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<ToDoListEventHandler>();

        [Function("ToDoListEventHandler")]  // TODO: change this name
        public async Task Run([CosmosDBTrigger(
            databaseName: "fancy-db",
            containerName: "ToDoListEventStream",
            Connection = "fancy-db",
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
