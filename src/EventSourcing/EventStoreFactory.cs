using EventSourcing.Core;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EventSourcing;

internal sealed class EventStoreFactory<T>(CosmosClient cosmosClient, 
    IOptionsMonitor<EventStoreOptions> namedOptionsAccessor, ILoggerFactory loggerFactory)
    : IEventStoreFactory<T> 
    public IEventStore<T> Create()
    {
        // TODO: Is this how we want to categorize logging??
        var logger = loggerFactory.CreateLogger($"{typeof(T)}.EventStore");

        // TODO Throw exception and test if doens't exist
        EventStoreOptions eventStoreOptions = namedOptionsAccessor.Get($"{aggregateType.Name}EventStore");
        
        var aggregateType = typeof(T).GetGenericArguments()[0];

        return new EventStore<T>(cosmosClient, eventStoreOptions, logger);
    }
}

public interface IEventStoreFactory<T>
    where T : AggregateRoot 
{
    IEventStore<T> Create();
}
