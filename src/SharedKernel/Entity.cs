using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;

namespace SharedKernel;

public abstract class Entity
{
   public Guid Id { get; protected set;  } = Guid.NewGuid();
   
   
   [JsonIgnore]
   public IReadOnlyList<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
   private readonly List<BaseDomainEvent> _domainEvents = [];

   private readonly Dictionary<Type, MethodInfo> _handlers = new();


   protected Entity()
   {
      // Get Handlers
      var handlers = this.GetType()
         .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
         .Where(w => w.Name.Equals("Handle"));

      // Add handlers to dictionary for faster lookup
      foreach (var handler in handlers)
      {
         var eventType = handler.GetParameters()
            .Single(s => s.ParameterType.IsSubclassOf(typeof(BaseDomainEvent)))
            .ParameterType;
         
         _handlers.Add(eventType, handler);
      }
   }

   
   protected List<BaseDomainEvent> CollectDomainEvents()
   {
      // Use Recursion & Reflection to navigate through all the properties on the entity
      // and retrieve their domain events
      var domainEvents = new List<BaseDomainEvent>(_domainEvents);
      
      // Get All Properties on the Entity
      var entityTypeProperties = this.GetType().GetProperties()
         .Where(w => w.PropertyType.IsSubclassOf(typeof(Entity)) || w.PropertyType.IsAssignableTo(typeof(IEnumerable<Entity>)));

      // Loop Through Properties
      // If it's an Enumerable Type, Loop Through Each Item Collecting It's Domain Events
      foreach (var entityTypeProperty in entityTypeProperties)
      {
         // If Enumerable
         var propertyValue = entityTypeProperty.GetValue(this)!;
         if (propertyValue.GetType().IsAssignableTo(typeof(IEnumerable)))
         {
            var entities = (IEnumerable<Entity>)propertyValue;
            domainEvents.AddRange(entities.SelectMany(s => s.CollectDomainEvents()).ToList());

            continue;
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
      // TODO: Use reflection to get correct "Handle" method - will be slower with lots of events
      // vs. dynamically calling "When" method - duplicate Apply & Mutate methods in every Aggregate/Entity
      // but potentially faster
      
      _handlers.TryGetValue(@event.GetType(), out MethodInfo? handler);
      handler?.Invoke(this, [@event]);

      // ((dynamic)this).When((dynamic)@event);
   }
}