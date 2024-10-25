using System.Text.Json;
using FancyToDo.Core.ToDoList;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using FancyToDo.Core.ToDoList.Entities.ToDoItem.DomainEvents;

namespace FancyToDo.UnitTests;

public class ToDoListsTests
{
    [Fact]
    public void CollectDomainEvents_ReturnsEventsInOrder()
    {
        // Given
        var toDoList = new ToDoList("Unit Test List");
        
        // When
        toDoList.AddToDo("Task 1");
        toDoList.RenameTask(toDoList.Items.Single().Id, "Rename Task 1");
        toDoList.AddToDo("Task 2");
        
        // Expectation
        
        // Verify ToDoList
        Assert.True(toDoList.Id != Guid.Empty);
        Assert.Equal("Unit Test List", toDoList.Name);
        
        Assert.True(toDoList.Items.Count == 2);
        
        // Verify First ToDoItem
        Assert.True(toDoList.Items[0].Id != Guid.Empty);
        Assert.Equal("Rename Task 1", toDoList.Items[0].Task);
        Assert.Equal("To Do", toDoList.Items[0].Status);
        
        // Verify Second ToDoItem
        Assert.True(toDoList.Items[1].Id != Guid.Empty);
        Assert.Equal("Task 2", toDoList.Items[1].Task);
        Assert.Equal("To Do", toDoList.Items[1].Status);

        // Verify DomainEvents
        var domainEvents = toDoList.CollectDomainEvents();
        
        Assert.Collection(domainEvents,
            e =>
            {
                Assert.IsType<ToDoListCreatedEvent>(e);
            },
            e =>
            {
                Assert.IsType<ItemAddedEvent>(e);
                
                var item = JsonSerializer.Deserialize<ToDoItem>(((ItemAddedEvent)e).Item)!;
                Assert.Equal("Task 1", item.Task);
            },
            e =>
            {
                Assert.IsType<TaskRenamedEvent>(e);
            },
            e =>
            {
                Assert.IsType<ItemAddedEvent>(e);
                
                var item = JsonSerializer.Deserialize<ToDoItem>(((ItemAddedEvent)e).Item)!;
                Assert.Equal("Task 2", item.Task);
            });
    }
}