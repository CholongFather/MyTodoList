using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/todos/{todoId:long}/links")]
public class LinksController(LinkService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(long todoId, CancellationToken ct)
        => Ok(await svc.GetByTodoIdAsync(todoId, ct));

    [HttpPost]
    public async Task<IActionResult> Create(long todoId, [FromBody] CreateLinkRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(todoId, req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateLinkRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
