using Ardalis.ApiEndpoints;
using EventSourcing.Core;
using FancyToDo.Core.ToDoList;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoItemEndpoints;

public record UpdateToDoItemRequest
{
    [FromRoute(Name = "id")] public Guid Id { get; init; }
    [FromRoute(Name = "itemId")] public Guid ItemId { get; init; }
    [FromBody] public RequestBody Body { get; init; }
};

public record RequestBody(string Task);

public class ToDoItemsUpdateEndpoint(IEventStore<ToDoList> eventStore) : EndpointBaseAsync
    .WithRequest<UpdateToDoItemRequest>
    .WithResult<IActionResult>
{
    [HttpPut("/todolists/{id}/todoitems/{itemId}")]
    [SwaggerOperation(
        Summary = "Updates a ToDo Item",
        Description = "Updates a ToDo Item",
        OperationId = "ToDoItem_Update",
        Tags = new[] { "ToDoItemEndpoint" })
    ] 
    public override async Task<IActionResult> HandleAsync(UpdateToDoItemRequest request, CancellationToken token)
    {
        var toDoList = await eventStore.TryLoad(request.Id);
        if (toDoList is null)
            throw new InvalidOperationException("ToDoList does not exist.");
        
        toDoList.RenameTask(request.ItemId, request.Body.Task);
        
        return NoContent();
    } 
}