using Ardalis.ApiEndpoints;
using EventSourcing;
using EventSourcing.Core;
using FancyToDo.Core.ToDoList;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoItemEndpoints;

public record CreateToDoItemRequest(Guid ListId, string Task);

public class ToDoItemsCreateEndpoint(IEventStore<ToDoList> eventStore) : EndpointBaseAsync
    .WithRequest<CreateToDoItemRequest>
    .WithResult<IActionResult>
{
    [HttpPost(Resources.ToDoItemRoute)]
    [SwaggerOperation(
        Summary = "Creates a ToDo Item",
        Description = "Creates a ToDo Item",
        OperationId = "ToDoItem_Create",
        Tags = new[] { "ToDoItemEndpoint" })
    ] 
    public override async Task<IActionResult> HandleAsync(CreateToDoItemRequest request, CancellationToken token)
    {
        // Load Aggregate
        var toDoList = await eventStore.Load(request.ListId);
        if (toDoList is null)
            throw new InvalidOperationException("ListId is Invalid");
        
        // Add new ToDoItem
        toDoList.AddToDo(request.Task);
        
        // Append Events that were raised during operations to Event Store
        await eventStore.Append(toDoList);
        
        return NoContent(); 
    }
}