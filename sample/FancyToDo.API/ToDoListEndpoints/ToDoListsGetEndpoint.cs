using Ardalis.ApiEndpoints;
using FancyToDo.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoListEndpoints;

public record GetToDoListResponse(Guid Id, string Name, List<GetToDoListsResponseToDoItem> Items);
public record GetToDoListsResponseToDoItem(Guid Id, string Task, string Status);

public class ToDoListsGetEndpoint(
    ToDoListReadOnlyRepository onlyRepository) : EndpointBaseAsync
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
        // Retrieve from Read Model/Materialized View
        // Could use the the CosmosClient, or abstract behind a read only repository
        // I like being able to enforce read only on the read side
        // TODO: Get rid of repository - vertical slices
        var toDoList = await onlyRepository.Get<GetToDoListResponse>()
            .ContinueWith(c => c.Result.FirstOrDefault(), token);
        
        if (toDoList is null)
            return BadRequest();

        return Ok(toDoList);
    }
}