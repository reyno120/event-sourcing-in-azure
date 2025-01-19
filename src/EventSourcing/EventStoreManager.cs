using EventSourcing.Core;

namespace EventSourcing;

internal sealed class EventStoreManager<TEventStore>(IEventStoreFactory<TEventStore> factory) 
    : IEventStore<TEventStore> where TEventStore : class 
{
    private volatile object? _syncObj;
    private volatile TEventStore? _store;
    public TEventStore Store
    {
        get
        {
            if (_store is TEventStore store)
                return store;
            
            // TODO: Research more
            lock (_syncObj ?? Interlocked.CompareExchange(ref _syncObj, new object(), null) ?? _syncObj)
                return _store ??= factory.Create();
        }
    }
}