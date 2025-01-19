using EventSourcing.Core;

namespace EventSourcing;

internal sealed class EventStoreManager<T>(IEventStoreFactory<T> factory) where T : AggregateRoot 
{
    private volatile object? _syncObj;
    private volatile IEventStore<T>? _store;
    public IEventStore<T> Store
    {
        get
        {
            if (_store is EventStore<T> store)
                return store;
            
            // TODO: Research more
            lock (_syncObj ?? Interlocked.CompareExchange(ref _syncObj, new object(), null) ?? _syncObj)
                return _store ??= factory.Create();
        }
    }
}