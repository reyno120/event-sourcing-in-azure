using System.Collections;

namespace SharedKernel;

public abstract class AggregateRoot : Entity 
{
    protected new void Mutate(BaseDomainEvent @event)
    {
        base.Mutate(@event);
    }

    public new List<BaseDomainEvent> CollectDomainEvents()
    {
        return base.CollectDomainEvents()
            .OrderBy(o => o.DateOccurred)
            .ToList();
    }
}