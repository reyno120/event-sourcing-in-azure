using Ardalis.ApiEndpoints;
using AutoMapper;
using FancyToDo.Core;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoListEndpoints;

public record GetToDoListResponse(Guid Id, string Name, List<GetToDoListsResponseToDoItem> Items);
public record GetToDoListsResponseToDoItem(Guid Id, string Task, string Status);

public class ToDoListsGetEndpoint(
    IMapper mapper,
    IEventStore eventStore) : EndpointBaseAsync
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
        var toDoList = await eventStore.Load<ToDoList>()
            .ContinueWith(c => c.Result.SingleOrDefault(), token);
        
        if (toDoList is null)
            return BadRequest();

        return Ok(mapper.Map<GetToDoListResponse>(toDoList));
    }
}