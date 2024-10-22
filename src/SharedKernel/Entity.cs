using System.Text.Json.Serialization;

namespace SharedKernel;

public abstract class Entity
{
   // [JsonPropertyName("id")]
   public Guid Id { get; protected set;  } = Guid.NewGuid();
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