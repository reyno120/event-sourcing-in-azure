using System.Reflection;
using System.Text.Json.Serialization;

namespace SharedKernel;

public abstract class Entity
{
   public Guid Id { get; protected set;  } = Guid.NewGuid();
   
   
   [JsonIgnore]
   public IReadOnlyList<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
   private readonly List<BaseDomainEvent> _domainEvents = [];

   
   protected List<BaseDomainEvent> CollectDomainEvents()
   {
      // Use Recursion & Reflection to navigate through all the properties on the entity
      // and retrieve their domain events
      var domainEvents = new List<BaseDomainEvent>(_domainEvents);
      
      var entityTypeProperties = this.GetType().GetProperties()
         .Where(w => w.PropertyType == typeof(Entity) || w.GetType().IsAssignableTo(typeof(IEnumerable<Entity>)));

      foreach (var entityTypeProperty in entityTypeProperties)
      {
         var propertyValue = entityTypeProperty.GetValue(this)!;
         if (propertyValue.GetType().IsAssignableTo(typeof(IEnumerable<Entity>)))
         {
            var entities = (IEnumerable<Entity>)propertyValue;
            domainEvents.AddRange(entities.SelectMany(s => s.CollectDomainEvents()).ToList());
         }

         var entity = (Entity)propertyValue;
         domainEvents.AddRange(entity.CollectDomainEvents());
      }

      return domainEvents;
   }

   public void ClearDomainEvents()
   {
      _domainEvents.Clear();
   }
   
   protected void Apply(BaseDomainEvent @event)
   {
      _domainEvents.Add(@event);
      Mutate(@event);
   }
    
   public void Mutate(BaseDomainEvent @event)
   {
      // TODO: Use reflection to get correct "When" method - will be slower with lots of events
      // vs. dynamically calling "When" method - duplicate Apply & Mutate methods in every Aggregate/Entity
      // but potentially faster
      this.GetType()
         .GetMethod("When", 
            BindingFlags.NonPublic | BindingFlags.Instance, 
            null, 
            CallingConventions.Any, 
            new[] { @event.GetType() }, 
            null)!
         .Invoke(this, [@event]);
      
      // ((dynamic)this).When((dynamic)@event);
   }
}