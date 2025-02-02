using System.Text;
using System.Text.Json;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Projections;
using Microsoft.Azure.Cosmos;

namespace EventSourcing.FunctionalTests;

[Collection("AppHost collection")]
public class ToDoItemsCreateEndpointTests(AppHostFixture fixture)
{
    [Fact]
    public async Task Create_Success()
    {
        // Arrange
        var toDoListId = Guid.NewGuid();
      
        EventStream stream = new
        (
            streamId: toDoListId,
            eventType: typeof(ToDoListCreatedEvent),
            version: 1,
            payload: JsonSerializer.Serialize(new ToDoListCreatedEvent(toDoListId, "Fancy ToDo List"))
        );
        await fixture.EventStreamContainer.UpsertItemAsync(stream);
        
        await fixture.ProjectionContainer.UpsertItemAsync(new
            { id = toDoListId.ToString(), name = "Fancy ToDo List", items = new List<ToDoItem>() });
        
        
        
        // Act
        using StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                ListId = toDoListId,
                Task = "Create_Success"
            }),
            Encoding.UTF8,
            "application/json");

        await fixture.FancyAPIClient.PostAsync("/todoitems", jsonContent);
        
        
        
        // Assert
        var toDoList = await fixture.ProjectionContainer
            .ReadManyItemsAsync<ToDoListView>(new List<(string id, PartitionKey partitionKey)> { (toDoListId.ToString(),  new PartitionKey("/id")) });
        
        // TODO: Also check eventstream saved correctly
        // Handle Azure Function Delay
        while (toDoList.First().Items.Count == 0)
        {
            await Task.Delay(500);
            toDoList = await fixture.ProjectionContainer
                .ReadManyItemsAsync<ToDoListView>(new List<(string id, PartitionKey partitionKey)> { (toDoListId.ToString(),  new PartitionKey("/id")) });
        }
        Assert.Equal("Create_Success", toDoList.First().Items.First().Task); 
        
    }
}