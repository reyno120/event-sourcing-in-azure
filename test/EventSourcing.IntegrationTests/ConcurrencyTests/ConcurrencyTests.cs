using System.Text.Json;
using FancyToDo.Core.ToDoList;
using FancyToDo.Core.ToDoList.DomainEvents;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventSourcing.IntegrationTests.ConcurrencyTests
{
	// Class Fixtures https://xunit.net/docs/shared-context
	// Tests WITHIN Class will not run in parallel https://xunit.net/docs/running-tests-in-parallel.html
	
	[Collection("CosmosDb collection")]
	public class ConcurrencyTests(CosmosDbTestContainerFixture fixture) : IAsyncLifetime 
	{
		private Container _container = null!;
		private EventStore<ToDoList> _eventStore = null!;
		
		public async Task InitializeAsync()
		{
			_container = await fixture.TestDatabase.DefineContainer("ConcurrencyTestContainer", "/streamId")
				.WithUniqueKey()
				.Path("/version")
				.Attach()
				.CreateIfNotExistsAsync();
			

			_eventStore = new EventStore<ToDoList>(_container, new Mock<ILogger<ToDoList>>().Object);
		}
		
		// Cheap easy way of clearing container between tests
		public async Task DisposeAsync() =>
			await _container.DeleteContainerAsync();
		
		
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

			await _container.CreateItemAsync(stream);
			

			// Act - Simulating save happening before process is finished
			var toDoList = await _eventStore.Load(_toDoListId);
			
			stream = new EventStream(
				streamId: _toDoListId,
				eventType: typeof(ItemAddedEvent),
				version: 2,
				payload: JsonSerializer.Serialize(
					new ItemAddedEvent(_toDoListId, Guid.NewGuid(), "Test Task 1", "To Do"))
			);
			await _container.CreateItemAsync(stream);	

			toDoList.AddToDo("Test Concurrent Task");
			
			
			// Assert - Validate Concurrency Error Occurs
			var exception = await Assert.ThrowsAsync<CosmosException>(async () => await _eventStore.Append(toDoList));
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

			await _container.CreateItemAsync(stream);
			

			// Act - Simulating save happening before process is finished
			var toDoList = await _eventStore.Load(_toDoListId);
			
			stream = new EventStream(
				streamId: _toDoListId,
				eventType: typeof(ItemAddedEvent),
				version: 2,
				payload: JsonSerializer.Serialize(
					new ItemAddedEvent(_toDoListId, Guid.NewGuid(), "Test Task 1", "To Do"))
			);
			await _container.CreateItemAsync(stream);	

			toDoList!.AddToDo("Test Concurrent Task");
			toDoList.RenameTask(toDoList.Items[0].Id, "Test Rename");

			
			// Assert - Validate Concurrency Error Occurs
			var exception = await Assert.ThrowsAsync<CosmosException>(async () => await _eventStore.Append(toDoList));
			Assert.Equal(HttpStatusCode.Conflict, exception.StatusCode);
		}
	}
}
