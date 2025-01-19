using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSourcing;

internal sealed class EventStoreFactory<TEventStore>(CosmosClient cosmosClient, 
    IOptionsMonitor<EventStoreOptions> namedOptionsAccessor, ILoggerFactory loggerFactory)
    : IEventStoreFactory<TEventStore> where TEventStore : class
{
    public TEventStore Create()
    {
        // TODO: Modify this if moving to separate repo
        var logger = loggerFactory.CreateLogger("EventStore"); 
        
        var aggregateType = typeof(TEventStore).GetGenericArguments()[0];
        
        return ((TEventStore)Activator.CreateInstance(typeof(TEventStore),
            cosmosClient, namedOptionsAccessor.Get($"{aggregateType.Name}EventStore"), logger)!)!;
        // TODO: Throw exception if accessor returns nothing?
    }
}

public interface IEventStoreFactory<TEventStore>
    where TEventStore : class
{
    TEventStore Create();
}
