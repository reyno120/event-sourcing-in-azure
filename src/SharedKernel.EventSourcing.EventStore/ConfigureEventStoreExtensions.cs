using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.EventSourcing.Core;

namespace SharedKernel.EventSourcing.EventStore;

public static class ConfigureEventStoreExtensions
{
    // TODO: Add annotations
    public static IHostApplicationBuilder ConfigureEventStore(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(typeof(IEventStore<>), typeof(EventStoreManager<>));
        builder.Services.AddTransient(typeof(IEventStoreFactory<>), typeof(EventStoreFactory<>));
        
        foreach (var section in builder.Configuration.GetSection("EventStores").GetChildren())
        {
            // TODO: Throw exception if empty configuration is found
            builder.Services.Configure<EventStoreOptions>(section.Key, section);  
        }


        return builder;
    }
}