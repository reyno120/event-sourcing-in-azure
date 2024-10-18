using FancyToDo.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace FancyToDo.Infrastructure;

public class EventStore(CosmosClient cosmosClient, string dbName, string containerName) : IEventStore
{
    private readonly Container _container = cosmosClient.GetContainer(dbName, containerName);

    
    public async Task<IEnumerable<T>> Load<T>()
    {
        var events = await LoadEvents();
        if (events.Count == 0) 
            return new List<T>();

        var streams = events
            .GroupBy(g => g.StreamId)
            .ToList();

        return streams
            .Select(stream => 
                ((T)Activator.CreateInstance(typeof(T), 
                    events.Select(s => s.Deserialize()))!)!)
            .ToList();
    }
    
    public async Task<T?> Load<T>(Guid id)
    {
        var events = await LoadEvents(id);
        if (events.Count == 0) 
            return default;

        return (T)Activator.CreateInstance(typeof(T), 
            events.Select(s => s.Deserialize()))!;
    }

    private async Task<List<EventStream>> LoadEvents(Guid? id = null)
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