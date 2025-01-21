using System.Text;
using System.Text.Json;
using EventSourcing;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Projections;
using Microsoft.Azure.Cosmos;

namespace FancyToDo.IntegrationTests;

public class ToDoItemsCreateEndpointTests 
{
    [Fact]
    public async Task Create_Valid()
    {
        // Arrange
        var cosmosClient = new CosmosClient(
            accountEndpoint: "https://localhost:8081/",
            authKeyOrResourceToken: "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            clientOptions: new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase } 
            }
        );

        var databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync("fancy-db");
        var eventContainer = await databaseResponse.Database.DefineContainer("ToDoListEventStream", "/streamId")
            .WithUniqueKey()
                .Path("/version")
                .Attach()
            .CreateIfNotExistsAsync();

        var toDoListId = Guid.NewGuid();
      
        EventStream stream = new
        (
            streamId: toDoListId,
            eventType: typeof(ToDoListCreatedEvent),
            version: 1,
            payload: JsonSerializer.Serialize(new ToDoListCreatedEvent(toDoListId, "Fancy ToDo List"))
        );
        await eventContainer.Container.UpsertItemAsync(stream);
        
        var projectionContainer = await databaseResponse.Database
            .CreateContainerIfNotExistsAsync(new ContainerProperties("ToDoLists", "/id"));
        await projectionContainer.Container.UpsertItemAsync(new
            { id = toDoListId.ToString(), name = "Fancy ToDo List", items = new List<ToDoItem>() });
        
        
        var httpClient = new HttpClient() { BaseAddress = new Uri("https://localhost:7180") };


        
        // Act
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                ListId = toDoListId,
                Task = "Create_Valid"
            }),
            Encoding.UTF8,
            "application/json");

        await httpClient.PostAsync("/todoitems", jsonContent);
        
        
        // Assert
        var toDoList = await projectionContainer.Container
            .ReadManyItemsAsync<ToDoListView>(new List<(string id, PartitionKey partitionKey)> { (toDoListId.ToString(),  new PartitionKey("/id")) });
        
        while (toDoList.First().Items.Count == 0)
        {
            await Task.Delay(500);
            toDoList = await projectionContainer.Container
                .ReadManyItemsAsync<ToDoListView>(new List<(string id, PartitionKey partitionKey)> { (toDoListId.ToString(),  new PartitionKey("/id")) });
        }
        Assert.Equal("Create_Valid", toDoList.First().Items.First().Task);
    }
}