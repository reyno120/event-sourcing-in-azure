// using Ardalis.ApiEndpoints;
// using FancyToDo.Core;
// using FancyToDo.Core.ToDoList;
// using FancyToDo.Core.ToDoList.DomainEvents;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace FancyToDo.API.ToDoItemEndpoints;
//
// public record CreateToDoItemRequest(Guid ListId, string Task);
//
// public class ToDoItemsCreateEndpoint(IEventStore eventStore) : EndpointBaseAsync
//     .WithRequest<CreateToDoItemRequest>
//     .WithResult<IActionResult>
// {
//     [HttpPost("/todoitems")]
//     [SwaggerOperation(
//         Summary = "Creates a ToDo Item",
//         Description = "Creates a ToDo Item",
//         OperationId = "ToDoItem_Create",
//         Tags = new[] { "ToDoItemEndpoint" })
//     ] 
//     public override async Task<IActionResult> HandleAsync(CreateToDoItemRequest request, CancellationToken token)
//     {
//         var toDoList = await eventStore.Load<ToDoList>(request.ListId);
//         if (toDoList is null)
//             throw new InvalidOperationException("ListId is Invalid");
//         
//         var newToDoItem = new ToDoItem(request.Task);
//         toDoList.AddToDo(newToDoItem);
//         
//         return NoContent(); 
//     }
// }