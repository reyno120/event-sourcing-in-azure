using System.Diagnostics;
using System.Text;
using System.Text.Json;
using FancyToDo.API.ToDoItemEndpoints;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Infrastructure;
using FancyToDo.Projections;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SharedKernel.EventSourcing.EventStore;

namespace FancyToDo.FunctionalTests;

// https://microsoft.github.io/AzureTipsAndTricks/blog/tip196.html

public class ToDoListTests : IDisposable
{
    private Process _process;
    private CosmosClient _cosmosClient;
    private IConfiguration _configuration;
    private EventStoreOptions _eventStoreOptions;
    
    [Fact]
    public async Task CreateToDoListItem()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _eventStoreOptions = new EventStoreOptions();
        _configuration.GetSection("EventStores:ToDoListEventStore")
            .Bind(_eventStoreOptions); 
        
        await InitializeDatabase();
        
        // StartAzureFunction();

        await MakeAPICall();
        
        await ValidateProjectionUpdated();
    }

    private async Task InitializeDatabase()
    {
        var clientOptions = new CosmosClientOptions()
        {
            SerializerOptions = new()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        _cosmosClient = new CosmosClient(
            _configuration.GetConnectionString("CosmosDBConnectionString"),
            clientOptions: clientOptions);

        var db = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_eventStoreOptions.DatabaseName);
        var eventStreamContainer = await db.Database.DefineContainer(_eventStoreOptions.ContainerName, "/streamId")
            .WithUniqueKey()
            .Path("/version")
            .Attach()
            .CreateIfNotExistsAsync();
        
        var projectionContainer = await db.Database.DefineContainer("ToDoLists", "/id")
            .CreateIfNotExistsAsync();
        
        
        // Seed Data
        var toDoListId = Guid.Parse("381cafbf-9126-43ff-bbd4-eda0eef17e97");
      
        EventStream stream = new
        (
            streamId: toDoListId,
            eventType: typeof(ToDoListCreatedEvent),
            version: 1,
            payload: JsonSerializer.Serialize(new ToDoListCreatedEvent(toDoListId, "Fancy ToDo List"))
        );
        await eventStreamContainer.Container.UpsertItemAsync(stream);
      
        await projectionContainer.Container.UpsertItemAsync(new
            { id = toDoListId.ToString(), name = "Fancy ToDo List", items = new List<ToDoItem>() });
    }

    private void StartAzureFunction()
    {
        _process = new Process
        {
            StartInfo = new ProcessStartInfo("func", 
                "host start --pause-on-error --dotnet-isolated-debug")
            {
                WorkingDirectory = _configuration.GetSection("WorkingDirectory").Value,
                // CreateNoWindow = false,
                // RedirectStandardOutput = true,
                UseShellExecute = false   //TODO: What is this for? - false allows you to redirect output, also don't have to search path for executable?
            }
        };


        // TODO: Move to fixture
        // TODO: Grab logs and exceptions and redirect to output
        // TODO: Make sure process is successfully disposed and not using port (Func.exe has stopped)
        // TODO: Figure out how to attach debugger
        _process.Start();
        Thread.Sleep(60000);

    }

    private async Task MakeAPICall()
    {
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost>();
        // await using var appHost = new TestingAspireAppHost();
        // appHost.Configuration.AddJsonFile("appsettings.json").Build();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
    
        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();
        


        using var httpClient = app.CreateHttpClient("fancy-api");
        await resourceNotificationService
            .WaitForResourceAsync("fancy-api", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));

        var postObject =
            new CreateToDoItemRequest(Guid.Parse("381cafbf-9126-43ff-bbd4-eda0eef17e97"), "Functional Test");
        
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(postObject),
            Encoding.UTF8,
            "application/json");


        await httpClient.PostAsync("/todoitems/", jsonContent);
        Thread.Sleep(15000);

    }

    private async Task ValidateProjectionUpdated()
    {
        var container = _cosmosClient.GetContainer(_eventStoreOptions.DatabaseName, "ToDoLists");
        
        IOrderedQueryable<ToDoListView> queryable = container.GetItemLinqQueryable<ToDoListView>();

        using FeedIterator<ToDoListView> linqFeed = queryable.ToFeedIterator();

        var records = new List<ToDoListView>();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<ToDoListView> response = await linqFeed.ReadNextAsync();
            records.AddRange(response);
        }

        var toDoList = records.Single();
        Assert.Equal("Functional Test", toDoList.Items.Single().Task);
    }

    public void Dispose()
    {
        if (!_process.HasExited)
            _process.Kill(entireProcessTree: true);
 
        _process.Dispose();
        
        Task.Run(async () => await _cosmosClient?.GetDatabase(_eventStoreOptions.DatabaseName).DeleteAsync()).Wait();
        _cosmosClient.Dispose();
    }
}


// public class TestingAspireAppHost() : DistributedApplicationFactory(typeof(Projects.AppHost))
// {
//     protected override void OnBuilderCreating(DistributedApplicationOptions applicationOptions,
//         HostApplicationBuilderSettings hostOptions)
//     {
//         hostOptions.Configuration.AddJsonFile("appsettings.json");
//     }
// }