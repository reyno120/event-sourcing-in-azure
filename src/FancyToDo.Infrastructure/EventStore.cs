using System.Text.Json;
using FancyToDo.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using SharedKernel;

namespace FancyToDo.Infrastructure;

public class EventStore(CosmosClient cosmosClient) : IEventStore
{
    // TODO: Make DatabaseName & ContainerName configurable
    private readonly Container _container = cosmosClient.GetContainer("fancy-db", "ToDoListEventStream");


    public async Task Append<T>(T aggregateRoot)
        where T : AggregateRoot 
    {
        // TODO: need to save version as row key so it throws an exception when trying to append same version
        // TODO: Handle exceptions and concurrency error
        var batch = _container.CreateTransactionalBatch(new PartitionKey(aggregateRoot.Id.ToString()));
        
        foreach (var domainEvent in aggregateRoot.CollectDomainEvents())
        {
            EventStream stream = new
            (
                streamId: aggregateRoot.Id,
                timeStamp: domainEvent.DateOccurred,    //TODO: This is being serialized in the payload
                eventType: domainEvent.GetType(),
                version: 1, // TODO: Implement versioning & handle concurrency
                payload: JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            );

            batch.CreateItem(stream);
        }

        await batch.ExecuteAsync();
        
        aggregateRoot.ClearDomainEvents();
    }
    
    public async Task<T?> Load<T>(Guid id) 
        where T : AggregateRoot 
    {
        var events = await LoadEvents(id);
        if (events.Count == 0) 
            return default;     // TODO: Throw exception instead of return default 

        return (T)Activator.CreateInstance(typeof(T), 
            events
                .Select(s => (BaseDomainEvent)s.Deserialize())
            )!;
    }

    private async Task<List<EventStream>> LoadEvents(Guid id)
    {
        // Query CosmosDB Event Stream
        // StreamId = AggregateId = Partition Key
        IOrderedQueryable<EventStream> queryable = _container.GetItemLinqQueryable<EventStream>();

        // TODO: Order by version
        var matches = queryable
            .Where(w => w.StreamId == id);

        using FeedIterator<EventStream> linqFeed = matches.ToFeedIterator();

        var events = new List<EventStream>();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<EventStream> response = await linqFeed.ReadNextAsync();
            events.AddRange(response);
        }

        return events;
    }
}