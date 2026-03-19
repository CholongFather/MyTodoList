using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CasesController(CaseService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] CaseStatus? status,
        [FromQuery] CaseCategory? category,
        [FromQuery] long? environmentId,
        [FromQuery] long? projectId,
        [FromQuery] string? search,
        CancellationToken ct)
        => Ok(await svc.GetAllAsync(status, category, environmentId, projectId, search, ct));

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] CaseStatus? status,
        [FromQuery] CaseCategory? category,
        [FromQuery] long? environmentId,
        [FromQuery] long? projectId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await svc.GetPagedAsync(status, category, environmentId, projectId, search, page, pageSize, ct));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await svc.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCaseRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateCaseRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateCaseStatusRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateStatusAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
        => Ok(await svc.GetStatsAsync(ct));

    // === 노트 ===

    [HttpPost("{caseId:long}/notes")]
    public async Task<IActionResult> CreateNote(long caseId, [FromBody] CreateCaseNoteRequest req, CancellationToken ct)
        => Ok(await svc.CreateNoteAsync(caseId, req, ct));

    [HttpPut("{caseId:long}/notes/{noteId:long}")]
    public async Task<IActionResult> UpdateNote(long caseId, long noteId, [FromBody] CreateCaseNoteRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateNoteAsync(noteId, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{caseId:long}/notes/{noteId:long}")]
    public async Task<IActionResult> DeleteNote(long caseId, long noteId, CancellationToken ct)
        => await svc.DeleteNoteAsync(noteId, ct) ? NoContent() : NotFound();

    // === 링크 ===

    [HttpPost("{caseId:long}/links")]
    public async Task<IActionResult> CreateLink(long caseId, [FromBody] CreateLinkRequest req, CancellationToken ct)
        => Ok(await svc.CreateLinkAsync(caseId, req, ct));

    [HttpPut("{caseId:long}/links/{linkId:long}")]
    public async Task<IActionResult> UpdateLink(long caseId, long linkId, [FromBody] CreateLinkRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateLinkAsync(linkId, req, ct);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpDelete("{caseId:long}/links/{linkId:long}")]
    public async Task<IActionResult> DeleteLink(long caseId, long linkId, CancellationToken ct)
        => await svc.DeleteLinkAsync(linkId, ct) ? NoContent() : NotFound();
}
