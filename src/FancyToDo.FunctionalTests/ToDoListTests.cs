using System.Text;
using System.Text.Json;
using FancyToDo.API.ToDoItemEndpoints;
using FancyToDo.Projections;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using SharedKernel.EventSourcing.EventStore;

namespace FancyToDo.FunctionalTests;

// public class TestingAspireAppHost : DistributedApplicationFactory(typeof(Projects.AppHost_ServiceTesting))
// {
//     protected override void OnBuilderCreating(DistributedApplicationOptions applicationOptions, HostApplicationBuilderSettings hostOptions)
//     {
//         builder.EnvironmentVariables
//     }
// }

// https://microsoft.github.io/AzureTipsAndTricks/blog/tip196.html

public class ToDoListTests
{
    private IConfiguration _configuration;
    private EventStoreOptions _eventStoreOptions;

    [Fact]
    public async Task CreateToDoListItem()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _eventStoreOptions = new EventStoreOptions();
        _configuration.GetSection("EventStores:ToDoListEventStore")
            .Bind(_eventStoreOptions);


        // TODO: Port for cosmos emulator is not static when running it through distributedapplicationtestingbuilder
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.AppHost_ServiceTesting>();
        // TODO: Need a way to pass directory path to apphost
        // https://learn.microsoft.com/en-us/dotnet/aspire/testing/manage-app-host?pivots=xunit#see-also
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        // TODO: Are logs being redirected to test output?
        await using var app = await appHost.BuildAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await app.StartAsync();

        
        using var httpClient = app.CreateHttpClient("fancy-api");
        
        // TODO: Not enough...health check to make sure function has actually started?
        await resourceNotificationService
            .WaitForResourceAsync("fancy-function", KnownResourceStates.Running);
        Thread.Sleep(15000);

        var postObject =
            new CreateToDoItemRequest(Guid.Parse("381cafbf-9126-43ff-bbd4-eda0eef17e97"), "Functional Test");

        using StringContent jsonContent = new(
            JsonSerializer.Serialize(postObject),
            Encoding.UTF8,
            "application/json");


        await httpClient.PostAsync("/todoitems/", jsonContent);
        Thread.Sleep(15000);


        var clientOptions = new CosmosClientOptions()
        {
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true,
            SerializerOptions = new()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        };
        // TODO: Best way to do this??
        // Configure port in appsettings maybe?

        var cosmosClient = new CosmosClient(
            await app.GetConnectionStringAsync("cosmosemulator"),
            clientOptions: clientOptions);

        var container = cosmosClient.GetContainer(_eventStoreOptions.DatabaseName, "ToDoLists");

        IOrderedQueryable<ToDoListView> queryable = container.GetItemLinqQueryable<ToDoListView>();

        // Create a waitfor extension b/c of azure function?
        using FeedIterator<ToDoListView> linqFeed = queryable.ToFeedIterator();

        var records = new List<ToDoListView>();
        while (linqFeed.HasMoreResults)
        {
            FeedResponse<ToDoListView> response = await linqFeed.ReadNextAsync();
            records.AddRange(response);
        }

        var toDoList = records.Single();
        Assert.Equal("Functional Test", toDoList.Items.Single().Task);

        cosmosClient.Dispose();
    }
}
