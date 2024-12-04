using System.Text.Json;
using FancyToDo.Core.ToDoList;
using FancyToDo.Core.ToDoList.DomainEvents;
using FancyToDo.Core.ToDoList.Entities.ToDoItem;
using Microsoft.Azure.Cosmos;
using SharedKernel;

namespace FancyToDo.IntegrationTests.Tests
{
	// Class Fixtures https://xunit.net/docs/shared-context
	// Tests WITHIN Class will not run in parallel https://xunit.net/docs/running-tests-in-parallel.html
	public class ConcurrencyTests(ConcurrencyFixture fixture) : IClassFixture<ConcurrencyFixture>, IDisposable
	{
		private readonly Guid _toDoListId = Guid.Parse("381cafbf-9126-43ff-bbd4-eda0eef17e97"); 
		
		[Fact]
		public async Task ConcurrencyTest_SingleEvent()
		{
			// Arrange
			// Seed initial event
			EventStream stream = new
			(
				streamId: _toDoListId,
				eventType: typeof(ToDoListCreatedEvent),
				version: 1,
				payload: JsonSerializer.Serialize(new ToDoListCreatedEvent(_toDoListId, "Fancy ToDo List"))
			);

			await fixture.EventStoreContainer.CreateItemAsync(stream);
			

			// Act
			var toDoList = await fixture.EventStore.Load<ToDoList>(_toDoListId);
			
			stream = new EventStream(
				streamId: _toDoListId,
				eventType: typeof(ItemAddedEvent),
				version: 2,
				payload: JsonSerializer.Serialize(new ItemAddedEvent(_toDoListId, new ToDoItem("Test Task")))
			);
			await fixture.EventStoreContainer.CreateItemAsync(stream);	

			toDoList!.AddToDo("Test Concurrent Task");

			
			// Assert
			var exception = await Assert.ThrowsAsync<CosmosException>(async () => await fixture.EventStore.Append(toDoList));
			Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
		}
		
		
		[Fact]
		public async Task ConcurrencyTest_MultipleEvents()
		{
			// Arrange
			// Seed initial event
			EventStream stream = new
			(
				streamId: _toDoListId,
				eventType: typeof(ToDoListCreatedEvent),
				version: 1,
				payload: JsonSerializer.Serialize(new ToDoListCreatedEvent(_toDoListId, "Fancy ToDo List"))
			);

			await fixture.EventStoreContainer.CreateItemAsync(stream);
			

			// Act
			var toDoList = await fixture.EventStore.Load<ToDoList>(_toDoListId);
			
			stream = new EventStream(
				streamId: _toDoListId,
				eventType: typeof(ItemAddedEvent),
				version: 2,
				payload: JsonSerializer.Serialize(new ItemAddedEvent(_toDoListId, new ToDoItem("Test Task")))
			);
			await fixture.EventStoreContainer.CreateItemAsync(stream);	

			toDoList!.AddToDo("Test Concurrent Task");
			toDoList.RenameTask(toDoList.Items[0].Id, "Test Rename");

			
			// Assert
			var exception = await Assert.ThrowsAsync<CosmosException>(async () => await fixture.EventStore.Append(toDoList));
			Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
		}

		public void Dispose()
		{
			Task.Run(async () => await fixture.ClearEventStore()).Wait();
		}
	}
}
