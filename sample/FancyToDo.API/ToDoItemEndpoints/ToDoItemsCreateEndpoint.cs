using Ardalis.ApiEndpoints;
using EventSourcing.Core;
using FancyToDo.Core.ToDoList;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoItemEndpoints;

public record CreateToDoItemRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }
    [FromBody] public CreateToDoItemRequestBody Body { get; init; } = null!;
};

public record CreateToDoItemRequestBody(string Task);

public class ToDoItemsCreateEndpoint(IEventStore<ToDoList> eventStore) : EndpointBaseAsync
    .WithRequest<CreateToDoItemRequest>
    .WithResult<IActionResult>
{
    [HttpPost("/todolists/{id}/todoitems")]
    [SwaggerOperation(
        Summary = "Creates a ToDo Item",
        Description = "Creates a ToDo Item",
        OperationId = "ToDoItem_Create",
        Tags = new[] { "ToDoItemEndpoint" })
    ] 
    public override async Task<IActionResult> HandleAsync(CreateToDoItemRequest request, CancellationToken token)
    {
        // Load Aggregate
        var toDoList = await eventStore.Load(request.Id);
        
        // Add new ToDoItem
        toDoList.AddToDo(request.Body.Task);
        
        // Append Events that were raised during operations to Event Store
        await eventStore.Append(toDoList);
        
        return NoContent(); 
    }
}