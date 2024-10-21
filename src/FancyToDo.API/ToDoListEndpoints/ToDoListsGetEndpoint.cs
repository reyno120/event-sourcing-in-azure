using Ardalis.ApiEndpoints;
using FancyToDo.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoListEndpoints;

public record GetToDoListResponse(Guid Id, string Name, List<GetToDoListsResponseToDoItem> Items);
public record GetToDoListsResponseToDoItem(Guid Id, string Task, string Status);

public class ToDoListsGetEndpoint(
    ToDoListReadRepository repository) : EndpointBaseAsync
    .WithoutRequest
    .WithResult<IActionResult>
{
    [HttpGet(Resources.ToDoListRoute)]
    [SwaggerOperation(
        Summary = "Gets all To Do Lists",
        Description = "Gets all To Do Lists",
        OperationId = "ToDoList_Get",
        Tags = new[] { "ToDoListEndpoint" })
    ]
    public override async Task<IActionResult> HandleAsync(CancellationToken token)
    {
        var toDoList = await repository.Get<GetToDoListResponse>()
            .ContinueWith(c => c.Result.SingleOrDefault(), token);
        
        if (toDoList is null)
            return BadRequest();

        // Projection - no need to map to DTO
        return Ok(toDoList);
    }
}