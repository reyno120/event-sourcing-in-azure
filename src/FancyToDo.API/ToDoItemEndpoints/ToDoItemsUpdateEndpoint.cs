// using Ardalis.ApiEndpoints;
// using FancyToDo.Core;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace FancyToDo.API.ToDoItemEndpoints;
//
// public record UpdateToDoItemRequest([FromRoute] Guid Id, [FromBody] string Task, [FromBody] string Status);
//
// public class ToDoItemsUpdateEndpoint : EndpointBaseAsync
//     .WithRequest<UpdateToDoItemRequest>
//     .WithResult<IActionResult>
// {
//     private readonly IBoardRepository _boardRepository;
//
//     public ToDoItemsUpdateEndpoint(IBoardRepository boardRepository)
//     {
//         _boardRepository = boardRepository;
//     }
//
//     [HttpPut("/todoitems/{id:Guid}")]
//     [SwaggerOperation(
//         Summary = "Updates a ToDo Item",
//         Description = "Updates a ToDo Item",
//         OperationId = "ToDoItem_Update",
//         Tags = new[] { "ToDoItemEndpoint" })
//     ] 
//     public override async Task<IActionResult> HandleAsync(UpdateToDoItemRequest request, CancellationToken token)
//     {
//         var board = await _boardRepository.Get();
//         
//         var item = board.ToDoLists
//             .SelectMany(s => s.Items)
//             .SingleOrDefault(s => s.Id == request.Id);
//         if (item is null)
//             throw new InvalidOperationException("ItemId is invalid.");
//         
//         item.RenameTask(request.Task);
//         item.SetStatus(request.Status);
//         
//         return NoContent();
//
//
//
//
//
//
//
//
//
//
//
//         /*var
//             purchaseOrder = _purchaseOrderRepository.Get(purchaseOrderId);
//             
//             var configuration = _someConfigurationOptionRepository.Get(configurationOptionId);
//             if(configuration is null) throw exception
//             
//             purchaseOrder.SetConfiguration(configuration)
//
//
//
//
//         */
//
//     } 
// }