using EventSourcing.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventSourcing;

public static class ConfigureEventStoreExtensions
{
    // TODO: Add annotations
    // TODO: Add optional parameter to define configuration section
    public static IHostApplicationBuilder ConfigureEventStore(this IHostApplicationBuilder builder)
    {
        // TODO: Make this an Azure CosmosDB specific extension and verify
        // container from config is setup correctly with correct paritionId & uniquekey?

        
        // Is there a test out there for memory leaks??
        builder.Services.AddTransient(typeof(IEventStore<>), typeof(EventStore<>));
        
        foreach (var section in builder.Configuration.GetSection("EventStores").GetChildren())
        {
            // TODO: Throw exception if empty configuration is found
            builder.Services.Configure<EventStoreOptions>(section.Key, section);  
        }


        return builder;
    }
}
