﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedKernel.EventSourcing.Core;

namespace SharedKernel.EventSourcing.EventStore;

public static class ConfigureEventStoreExtensions
{
    // TODO: Add annotations
    public static IHostApplicationBuilder ConfigureEventStore(this IHostApplicationBuilder builder, 
        params Assembly[] assembliesToScan)
    {
        builder.Services.AddSingleton(typeof(IEventStore<>), typeof(EventStoreManager<>));
        builder.Services.AddTransient(typeof(IEventStoreFactory<>), typeof(EventStoreFactory<>));
        
        
        // Scan Assembly for Event Stores
        var eventStoreTypes = assembliesToScan
            .SelectMany(s => s.DefinedTypes)
            .EventStores();

        
        foreach (var eventStoreType in eventStoreTypes)
        {
            string section = $"EventStores:{eventStoreType.Name}";
            builder.Services.Configure<EventStoreOptions>(eventStoreType.Name,
                builder.Configuration.GetSection(section)); // TODO: Throw exception if empty configuration is found
        }
        
        
        return builder;
    }


    private static IEnumerable<Type> EventStores(this IEnumerable<TypeInfo> types)
    {
        return types.Where(s => 
            s.BaseGenericTypes()
                .Any(a => a.GetGenericTypeDefinition() == typeof(EventStore<>))
            );
    }

    private static IEnumerable<Type> BaseGenericTypes(this Type type)
    {
        Type t = type;
        while (true)
        {
            t = t.BaseType;
            if (t == null) break;
            
            if (t.IsGenericType)
                yield return t;
        }
    }
}