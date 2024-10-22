using System.Text.Json;
using FancyToDo.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using SharedKernel;

namespace FancyToDo.Infrastructure;

public class EventStore(CosmosClient cosmosClient, string dbName, string containerName) : IEventStore
{
    private readonly Container _container = cosmosClient.GetContainer(dbName, containerName);


    public async Task Append<T>(T aggregateRoot)
        where T : AggregateRoot 
    {
        // TODO: need to save version as row key so it throws an exception when trying to append same version
        // TODO: Handle exceptions and concurrency error
        var batch = _container.CreateTransactionalBatch(new PartitionKey(aggregateRoot.Id.ToString()));
        
        foreach (var domainEvent in aggregateRoot.DomainEvents)
        {
            EventStream stream = new()
            {
                StreamId = aggregateRoot.Id,
                TimeStamp = domainEvent.DateOccurred,
                EventType = domainEvent.GetType(),
                Payload = JsonSerializer.Serialize(domainEvent)
            };

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
            return default;

        return (T)Activator.CreateInstance(typeof(T), 
            events.Select(s => s.Deserialize()))!;
    }

    private async Task<List<EventStream>> LoadEvents(Guid id)
    {
        IOrderedQueryable<EventStream> queryable = _container.GetItemLinqQueryable<EventStream>();

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