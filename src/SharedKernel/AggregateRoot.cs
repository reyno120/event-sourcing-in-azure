using System.Text.Json.Serialization;

namespace SharedKernel;

public abstract class AggregateRoot : Entity 
{
   private readonly List<BaseDomainEvent> _domainEvents = [];
   public IReadOnlyList<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

   public void AddDomainEvent(BaseDomainEvent domainEvent)
   {
      _domainEvents.Add(domainEvent);
   }

   public void ClearDomainEvents()
   {
      _domainEvents.Clear();
   }
}