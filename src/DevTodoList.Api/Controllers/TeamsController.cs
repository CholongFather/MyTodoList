using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

/// <summary>팀/작업분류 관리 API</summary>
[ApiController]
[Route("api/[controller]")]
public class TeamsController(TeamService svc) : ControllerBase
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
    public async Task<IActionResult> Create([FromBody] CreateTeamRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateTeamRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    // === 작업 분류 ===

    [HttpPost("{teamId:long}/categories")]
    public async Task<IActionResult> CreateCategory(long teamId, [FromBody] CreateWorkCategoryRequest req, CancellationToken ct)
        => Ok(await svc.CreateCategoryAsync(teamId, req, ct));

    [HttpPut("categories/{id:long}")]
    public async Task<IActionResult> UpdateCategory(long id, [FromBody] CreateWorkCategoryRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateCategoryAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("categories/{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id, CancellationToken ct)
        => await svc.DeleteCategoryAsync(id, ct) ? NoContent() : NotFound();
}
