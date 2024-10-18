using Ardalis.ApiEndpoints;
using FancyToDo.Core;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoItemEndpoints;

public record CreateToDoItemRequest(Guid ListId, string Task);

public class ToDoItemsCreateEndpoint : EndpointBaseAsync
    .WithRequest<CreateToDoItemRequest>
    .WithResult<IActionResult>
{
    private readonly IEventStore _eventStore;

    public ToDoItemsCreateEndpoint(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    [HttpPost("/todoitems")]
    [SwaggerOperation(
        Summary = "Creates a ToDo Item",
        Description = "Creates a ToDo Item",
        OperationId = "ToDoItem_Create",
        Tags = new[] { "ToDoItemEndpoint" })
    ] 
    public override async Task<IActionResult> HandleAsync(CreateToDoItemRequest request, CancellationToken token)
    {
        var board = await _boardRepository.Get();
        var list = board.ToDoLists.SingleOrDefault(s => s.Id == request.ListId);
        if (list is null)
            throw new InvalidOperationException("ListId is Invalid");
        
        var newToDoItem = new ToDoItem(request.Task);
        list.AddToDo(newToDoItem);
        
        return NoContent(); 
    }
}