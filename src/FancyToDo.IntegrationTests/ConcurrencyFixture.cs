﻿using FancyToDo.Infrastructure;
using FancyToDo.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace FancyToDo.IntegrationTests;

public class ConcurrencyFixture : IDisposable 
{
    private readonly EventStoreOptions _eventStoreOptions;
    private readonly CosmosClient _cosmosClient;
    public readonly EventStore EventStore;
    public Container EventStoreContainer { get; private set; }
    
    public ConcurrencyFixture()
    {
        // Configure
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _eventStoreOptions = new EventStoreOptions();
        config.GetSection(EventStoreOptions.EventStore).Bind(_eventStoreOptions);

        
        // Initialize CosmosDB Emulator
        var clientOptions = new CosmosClientOptions()
        {
            SerializerOptions = new()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        _cosmosClient = new CosmosClient(
            accountEndpoint: "https://localhost:8081/",
            authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            clientOptions: clientOptions 
            );
        
        
        // Initialize DB & Container
        Task.Run(async () =>
        {
            var db = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_eventStoreOptions.DatabaseName);
            await CreateContainer(db.Database);
        }).Wait();
        
        
        // Initialize EventStore
        var mockEventStoreOptions = new Mock<IOptions<EventStoreOptions>>();
        mockEventStoreOptions.SetupGet(s => s.Value)
                .Returns(_eventStoreOptions);
        
        EventStore = new EventStore(_cosmosClient, mockEventStoreOptions.Object);
    }

    private async Task CreateContainer(Database db)
    {
        this.EventStoreContainer = await db.DefineContainer(_eventStoreOptions.ContainerName, "/streamId")
            .WithUniqueKey()
            .Path("/version")
            .Attach()
            .CreateIfNotExistsAsync(); 
    }
    
    public async Task ClearEventStore()
    {
        // Cheap Easy way of clearing Event Store - Drop/Re-Create
        var db = _cosmosClient.GetDatabase(_eventStoreOptions.DatabaseName);
        await db.GetContainer(_eventStoreOptions.ContainerName).DeleteContainerAsync();

        await CreateContainer(db);
    }

    public void Dispose()
    {
        // Cleanup - Remove Test Database
        var database = _cosmosClient.GetDatabase(_eventStoreOptions.DatabaseName);
        Task.Run(async () => await database.DeleteAsync()).Wait();
        
        _cosmosClient.Dispose();
    }
}