﻿using Ardalis.ApiEndpoints;
using FancyToDo.Core.ToDoList;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.EventSourcing.Core;
using SharedKernel.EventSourcing.EventStore;
using Swashbuckle.AspNetCore.Annotations;

namespace FancyToDo.API.ToDoItemEndpoints;

public record UpdateToDoItemRequest([FromRoute] Guid Id, [FromRoute] Guid TaskId, [FromBody] string Task, [FromBody] string Status);

public class ToDoItemsUpdateEndpoint(IEventStore<EventStore<ToDoList>> eventStore) : EndpointBaseAsync
    .WithRequest<UpdateToDoItemRequest>
    .WithResult<IActionResult>
{
    private readonly EventStore<ToDoList> _eventStore = eventStore.Store;
    
    [HttpPut("/todolists/{id:Guid}/todoitems/{itemId:Guid}")]
    [SwaggerOperation(
        Summary = "Updates a ToDo Item",
        Description = "Updates a ToDo Item",
        OperationId = "ToDoItem_Update",
        Tags = new[] { "ToDoItemEndpoint" })
    ] 
    public override async Task<IActionResult> HandleAsync(UpdateToDoItemRequest request, CancellationToken token)
    {
        var toDoList = await _eventStore.Load(request.Id);
        if (toDoList is null)
            throw new InvalidOperationException("ToDoList does not exist.");
        
        toDoList.RenameTask(request.TaskId, request.Task);
        
        return NoContent();
    } 
}