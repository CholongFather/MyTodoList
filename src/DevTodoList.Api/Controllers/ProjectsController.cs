using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(ProjectService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await svc.GetAllAsync(ct));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await svc.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateProjectRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequest req, CancellationToken ct)
    {
        await svc.ReorderAsync(req, ct);
        return NoContent();
    }

    // === 프로젝트 메타 (URL/버전/비밀번호) ===

    [HttpPost("{projectId:long}/metas")]
    public async Task<IActionResult> CreateMeta(long projectId, [FromBody] CreateProjectMetaRequest req, CancellationToken ct)
        => Ok(await svc.CreateMetaAsync(projectId, req, ct));

    [HttpPut("metas/{metaId:long}")]
    public async Task<IActionResult> UpdateMeta(long metaId, [FromBody] CreateProjectMetaRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateMetaAsync(metaId, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("metas/{metaId:long}")]
    public async Task<IActionResult> DeleteMeta(long metaId, CancellationToken ct)
        => await svc.DeleteMetaAsync(metaId, ct) ? NoContent() : NotFound();
}
