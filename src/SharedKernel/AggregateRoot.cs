using System.Collections;

namespace SharedKernel;

public abstract class AggregateRoot : Entity
{
    protected int _version { get; init; } = 0;
    public int Version => _version;

    protected AggregateRoot(IEnumerable<BaseDomainEvent> events)
    {
        foreach (var @event in events)
        { 
            _version++;
            base.Mutate(@event);
        }
    }
    
    protected AggregateRoot() {}

    public new List<BaseDomainEvent> CollectDomainEvents()
    {
        return base.CollectDomainEvents()
            .OrderBy(o => o.DateOccurred)
            .ToList();
    }
}