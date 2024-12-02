using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.EventSourcing.Core;

namespace SharedKernel.EventSourcing.EventStore;

public static class ConfigureEventStoreExtensions
{
    public static WebApplicationBuilder ConfigureEventStore(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<EventStoreOptions>(builder.Configuration.GetSection(EventStoreOptions.EventStore));
        builder.Services.AddSingleton<IEventStore, EventStore>();

        return builder;
    }
    
    public static FunctionsApplicationBuilder ConfigureEventStore(this FunctionsApplicationBuilder builder)
    {
        builder.Services.Configure<EventStoreOptions>(builder.Configuration.GetSection(EventStoreOptions.EventStore));
        builder.Services.AddSingleton<IEventStore, EventStore>();

        return builder;
    }
}