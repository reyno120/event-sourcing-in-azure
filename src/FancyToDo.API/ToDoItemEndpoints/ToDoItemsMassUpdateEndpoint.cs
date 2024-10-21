// using Ardalis.ApiEndpoints;
// using FancyToDo.Core;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace FancyToDo.API.ToDoItemEndpoints;
//
// public record MassUpdateToDoItemRequest([FromQuery] Guid[] Ids, [FromBody] string Status);
//
// public class ToDoItemsMassUpdateEndpoint : EndpointBaseAsync
//     .WithRequest<MassUpdateToDoItemRequest>
//     .WithResult<IActionResult>
// {
//     private readonly IBoardRepository _boardRepository;
//
//     public ToDoItemsMassUpdateEndpoint(IBoardRepository boardRepository)
//     {
//         _boardRepository = boardRepository;
//     }
//
//     [HttpPut("/todoitems")]
//     [SwaggerOperation(
//         Summary = "Mass Updates a ToDo Item",
//         Description = "Mass Updates a ToDo Item",
//         OperationId = "ToDoItem_MassUpdate",
//         Tags = new[] { "ToDoItemEndpoint" })
//     ]
//     public override async Task<IActionResult> HandleAsync(MassUpdateToDoItemRequest request, CancellationToken token)
//     {
//         var board = await _boardRepository.Get();
//         
//         var items = board.ToDoLists
//             .SelectMany(s => s.Items)
//             .Where(s => request.Ids.Contains(s.Id));
//
//         foreach (var item in items)
//         {
//             item.SetStatus(request.Status);
//         }
//         
//         return NoContent(); 
//     }
// }