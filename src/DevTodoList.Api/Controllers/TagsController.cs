using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController(TagService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await svc.GetAllAsync(ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateTagRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
