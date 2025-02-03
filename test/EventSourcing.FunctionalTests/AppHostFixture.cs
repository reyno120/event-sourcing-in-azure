using Aspire.Hosting;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using FancyToDo.API.Configuration;
using FancyToDo.Core.ToDoList;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace EventSourcing.FunctionalTests;

public class AppHostFixture : IAsyncLifetime
{
    public Container EventStreamContainer { get; private set; } = null!;
    public Container ProjectionContainer { get; private set; } = null!;
    public HttpClient FancyAPIClient { get; private set; } = null!;
    
    private DistributedApplication _app = null!;
    private CosmosClient _cosmosClient = null!;
    private IFutureDockerImage _azureFunctionImage = null!;
    private IContainer _azureFunctionContainer = null!;
    
    public async Task InitializeAsync()
    {
        // SETUP APPHOST
        // https://learn.microsoft.com/en-us/dotnet/aspire/testing/manage-app-host?pivots=xunit#see-also
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.FancyToDo_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        
        _app = await appHost.BuildAsync();
        var resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
        await _app.StartAsync();
        
        
        // SETUP API HTTP CLIENT
        FancyAPIClient = _app.CreateHttpClient("fancy-api");
        await resourceNotificationService.WaitForResourceAsync("fancy-api", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));
        
        
        
        // OBTAIN CONFIGURATION

        var fancyApiSettings = Path.Combine(Directory.GetCurrentDirectory(),
            "..\\..\\..\\..\\..\\sample\\FancyToDo.API\\appsettings.json");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(fancyApiSettings).Build();
        
        var eventStoreOptions = new EventStoreOptions();
        configuration.GetSection($"EventStores:{nameof(ToDoList)}").Bind(eventStoreOptions);

        var projectionOptions = new ProjectionOptions();
        configuration.GetSection("Projection").Bind(projectionOptions);
        
        // SETUP AZURE FUNCTION TEST CONTAINER
        // At this time, Aspire does not support CosmosDBTriggered Azure Functions
        _azureFunctionImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("sample/FancyToDo.Functions/Dockerfile")
            .Build();
        await _azureFunctionImage.CreateAsync().ConfigureAwait(false);

        _azureFunctionContainer = new ContainerBuilder().WithImage(_azureFunctionImage)
            .WithEnvironment("DatabaseName", eventStoreOptions.DatabaseName)
            .WithEnvironment("ContainerName", eventStoreOptions.ContainerName)
            .WithEnvironment("CosmosDBConnectionString", 
                await _app.GetConnectionStringAsync("fancy-cosmos"))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Worker process started and initialized.",
                o => o.WithTimeout(TimeSpan.FromMinutes(3))))
            .Build();
        // TODO: ConfigureAwait?
        await _azureFunctionContainer.StartAsync().ConfigureAwait(false);
        
        
        // SETUP COSMOS CLIENT
        var clientOptions = new CosmosClientOptions()
        {
            ConnectionMode = ConnectionMode.Gateway,
            SerializerOptions = new()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        _cosmosClient = new CosmosClient(
            await _app.GetConnectionStringAsync("fancy-cosmos"),
            clientOptions: clientOptions);

        EventStreamContainer =
            _cosmosClient.GetContainer(eventStoreOptions.DatabaseName, eventStoreOptions.ContainerName);
        ProjectionContainer =
            _cosmosClient.GetContainer(projectionOptions.DatabaseName, eventStoreOptions.ContainerName);
    }

    public async Task DisposeAsync()
    {
        await _azureFunctionContainer.DisposeAsync();
        await _azureFunctionContainer.DisposeAsync();
        FancyAPIClient.Dispose();
        await _app.DisposeAsync();
        _cosmosClient.Dispose();
    }
}

[CollectionDefinition("AppHost collection")]
public class AppHostCollection : ICollectionFixture<AppHostFixture> {}