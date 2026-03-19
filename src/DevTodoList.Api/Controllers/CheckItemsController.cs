using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/todos/{todoId:long}/checkitems")]
public class CheckItemsController(CheckItemService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(long todoId, CancellationToken ct)
        => Ok(await svc.GetByTodoIdAsync(todoId, ct));

    [HttpPost]
    public async Task<IActionResult> Create(long todoId, [FromBody] CreateCheckItemRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(todoId, req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateCheckItemRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:long}/toggle")]
    public async Task<IActionResult> Toggle(long id, CancellationToken ct)
        => await svc.ToggleAsync(id, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(long todoId, [FromBody] ReorderRequest req, CancellationToken ct)
    {
        await svc.ReorderAsync(todoId, req, ct);
        return NoContent();
    }
}
