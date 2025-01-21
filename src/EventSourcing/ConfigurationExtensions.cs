using EventSourcing.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// TODO: Future Enhancement - Mark extension and EventStore specifically for Azure CosmosDB NoSQL
// and add implementations for other technologies

namespace EventSourcing;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers event store configuration values as an EventStoreOptions NamedOption
    /// </summary>
    /// <param name="builder">The working Host Application Builder</param>
    /// <param name="configurations">Optional configuration section. Default is "EventStores"</param>
    /// <returns>The working Host Application Builder</returns>
    /// <exception cref="ArgumentException">Thrown when missing a required configuration value</exception>
    public static IHostApplicationBuilder ConfigureEventStores(this IHostApplicationBuilder builder, 
        IConfigurationSection? configurations = null)
    {
        // TODO: Is there a test out there for memory leaks??
        builder.Services.AddTransient(typeof(IEventStoreFactory<>), typeof(EventStoreFactory<>));
        builder.Services.AddSingleton(typeof(IEventStore<>), typeof(EventStoreManager<>));
        
        
        var eventStoreConfigurations = configurations?.GetChildren() ?? 
                                       builder.Configuration.GetSection("EventStores").GetChildren();

        var eventStoreOptionsProperties = typeof(EventStoreOptions).GetProperties();
        foreach (var section in eventStoreConfigurations)
        {
            foreach (var property in eventStoreOptionsProperties)
                _ = section[property.Name] ??
                    throw new ArgumentException($"{property.Name} is missing from {section.Key} event store configuration.");
            
            builder.Services.Configure<EventStoreOptions>(section.Key, section);
        }
        

        return builder;
    }

    public static IHost VerifyEventStoreConfigurations(this IHost app)
    {
        // TODO: You can't change the partition key or unique key policy of a container once it's created
        // might be helpful to have a verification at program build to catch issues quicker instead of at runtime
        return app;
    }
}
