using Microsoft.Azure.Cosmos;
using Testcontainers.CosmosDb;

namespace EventSourcing.IntegrationTests;

public class CosmosDbTestContainerFixture : IAsyncLifetime
{
    private readonly CosmosDbContainer _cosmosDbContainer = new CosmosDbBuilder().Build();
    private CosmosClient _cosmosClient = null!;
    public Database TestDatabase { get; private set; } = null!;
    private const string DatabaseName = "ConcurrencyTestDatabase";


    public async Task InitializeAsync()
    {
        // https://github.com/testcontainers/testcontainers-dotnet/blob/a0f1f7694b4602aa2ba7da6f167ddc4a56670ecc/tests/Testcontainers.CosmosDb.Tests/CosmosDbContainerTest.cs#L26
        await _cosmosDbContainer.StartAsync();
        
        // Get CosmosDB Client
        var clientOptions = new CosmosClientOptions()
        {
            SerializerOptions = new()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            },
            ConnectionMode = ConnectionMode.Gateway,
            HttpClientFactory = () => _cosmosDbContainer.HttpClient
        };
        _cosmosClient = new CosmosClient(_cosmosDbContainer.GetConnectionString(),
            clientOptions: clientOptions 
        );
        
        
        // Create Database
        TestDatabase = await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);
    }

    public async Task DisposeAsync()
    {
        // Cleanup - Remove Test Database
        await TestDatabase.DeleteAsync();
        _cosmosClient.Dispose();
    }
}

[CollectionDefinition("CosmosDb collection")]
public class CosmosDbTestContainerCollection : ICollectionFixture<CosmosDbTestContainerFixture> {}