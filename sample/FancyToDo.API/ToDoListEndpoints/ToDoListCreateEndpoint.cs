using Ardalis.ApiEndpoints;
using EventSourcing.Core;
using FancyToDo.Core.ToDoList;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoListEndpoints;

public record CreateToDoListRequest(string Name);

public class ToDoListCreateEndpoint(IEventStore<ToDoList> eventStore) : EndpointBaseAsync
    .WithRequest<CreateToDoListRequest>
    .WithActionResult
{
    [HttpPost("/todolists")]
    [SwaggerOperation(
        Summary = "Creates a new ToDo List",
        Description = "Creates a new ToDo List",
        OperationId = "ToDoList_Create",
        Tags = new[] { "ToDoListEndpoints" })
    ]
    public override async Task<ActionResult> HandleAsync(CreateToDoListRequest request, CancellationToken cancellationToken)
    {
        await eventStore.Append(new ToDoList(request.Name));
        return Created();
    }
}