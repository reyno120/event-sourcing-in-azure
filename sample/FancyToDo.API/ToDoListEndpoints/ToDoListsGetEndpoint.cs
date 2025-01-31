using Ardalis.ApiEndpoints;
using FancyToDo.API.Configuration;
using FancyToDo.API.Extensions;
using FancyToDo.Projections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoListEndpoints;

public record GetToDoListResponse(Guid Id, string Name, List<GetToDoListsResponseToDoItem> Items);
public record GetToDoListsResponseToDoItem(Guid Id, string Task, string Status);

public class ToDoListsGetEndpoint(
    CosmosClient cosmosClient, 
    IOptions<ProjectionOptions> options) : EndpointBaseAsync
    .WithoutRequest
    .WithResult<IActionResult>
{
    [HttpGet("/todolists")]
    [SwaggerOperation(
        Summary = "Gets all To Do Lists",
        Description = "Gets all To Do Lists",
        OperationId = "ToDoList_Get",
        Tags = new[] { "ToDoListEndpoint" })
    ]
    public override async Task<IActionResult> HandleAsync(CancellationToken token)
    {
        var container = cosmosClient
            .GetContainer(options.Value.DatabaseName, options.Value.ContainerName); 

        return Ok(await container.Get<ToDoListView>());
    }
}