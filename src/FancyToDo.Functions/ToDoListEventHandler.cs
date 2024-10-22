using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace FancyToDo.Functions
{
    public class ToDoListEventHandler
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public ToDoListEventHandler(ILoggerFactory loggerFactory, IMediator mediator)
        {
            _logger = loggerFactory.CreateLogger<ToDoListEventHandler>();
            _mediator = mediator;
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
                await _mediator.Publish(stream.Deserialize());
            
            
            
            
            // foreach (var stream in input)
            // {
            //     var handler = (IEventHandler)Activator.CreateInstance(
            //         Type.GetType($"{stream.EventType.ToString()}Handler"),
            //         // new object[] { _cosmosClient }
            //         new object[] { }
            //     );
            //
            //     if (handler is null)
            //         return;
            //
            //     await handler.Handle(stream.Deserialize());
            // }
        }
    }
}
