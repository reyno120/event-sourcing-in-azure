namespace SharedKernel;

public abstract class Entity
{
   public Guid Id { get; protected set;  }
   private readonly List<IEvent> _domainEvents = [];
   public IReadOnlyList<IEvent> DomainEvents => _domainEvents.AsReadOnly();

   public void AddDomainEvent(IEvent domainEvent)
   {
      _domainEvents.Add(domainEvent);
   }

   public void ClearDomainEvents()
   {
      _domainEvents.Clear();
   }
}