using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController(TodoService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] long? projectId, [FromQuery] int? status,
        [FromQuery] string? tagIds, [FromQuery] int? priority,
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] string? search,
        [FromQuery] long? teamId, [FromQuery] int? assigneeType, [FromQuery] long? workCategoryId,
        [FromQuery] long? assigneeTypeId,
        CancellationToken ct)
        => Ok(await svc.GetFilteredAsync(projectId, status, tagIds, priority, fromDate, toDate, search, teamId, assigneeType, workCategoryId, assigneeTypeId, ct));

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] long? projectId, [FromQuery] int? status,
        [FromQuery] string? tagIds, [FromQuery] int? priority,
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] string? search,
        [FromQuery] long? teamId, [FromQuery] int? assigneeType, [FromQuery] long? workCategoryId,
        [FromQuery] long? assigneeTypeId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await svc.GetPagedAsync(projectId, status, tagIds, priority, fromDate, toDate, search, teamId, assigneeType, workCategoryId, page, pageSize, assigneeTypeId, ct));

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ct)
    {
        var result = await svc.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(req, ct));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTodoRequest req, CancellationToken ct)
    {
        var result = await svc.UpdateAsync(id, req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPatch("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStatusRequest req, CancellationToken ct)
        => await svc.UpdateStatusAsync(id, req.Status, ct) ? NoContent() : NotFound();

    /// <summary>태그 즉시 동기화</summary>
    [HttpPatch("{id:long}/tags")]
    public async Task<IActionResult> UpdateTags(long id, [FromBody] UpdateTagsRequest req, CancellationToken ct)
        => await svc.UpdateTagsAsync(id, req.TagIds, ct) ? NoContent() : NotFound();

    /// <summary>작업자 즉시 동기화</summary>
    [HttpPatch("{id:long}/workers")]
    public async Task<IActionResult> UpdateWorkers(long id, [FromBody] UpdateWorkersRequest req, CancellationToken ct)
        => await svc.UpdateWorkersAsync(id, req.WorkerIds, ct) ? NoContent() : NotFound();

    /// <summary>담당 유형 즉시 변경</summary>
    [HttpPatch("{id:long}/assignee-type")]
    public async Task<IActionResult> UpdateAssigneeType(long id, [FromBody] UpdateAssigneeTypeRequest req, CancellationToken ct)
        => await svc.UpdateAssigneeTypeAsync(id, (int)req.AssigneeType, req.AssigneeTypeId, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    [HttpPost("{id:long}/duplicate")]
    public async Task<IActionResult> Duplicate(long id, CancellationToken ct)
    {
        var result = await svc.DuplicateAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ReorderRequest req, CancellationToken ct)
    {
        await svc.ReorderAsync(req, ct);
        return NoContent();
    }

    /// <summary>CSV 내보내기</summary>
    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportCsv(
        [FromQuery] long? projectId, [FromQuery] int? status,
        [FromQuery] long? teamId, [FromQuery] int? assigneeType,
        CancellationToken ct)
    {
        var todos = await svc.GetFilteredAsync(projectId, status, null, null, null, null, null, teamId, assigneeType, null, null, ct);
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("제목,프로젝트,상태,우선순위,팀,담당유형,마감일,시작일,종료일,태그");
        foreach (var t in todos)
        {
            var statusLabel = t.Status switch { TodoStatus.Todo => "할일", TodoStatus.InProgress => "진행중", TodoStatus.Done => "완료", TodoStatus.Archived => "보관", _ => t.Status.ToString() };
            var priorityLabel = t.Priority switch { TodoPriority.Low => "낮음", TodoPriority.Normal => "보통", TodoPriority.High => "높음", TodoPriority.Urgent => "긴급", _ => t.Priority.ToString() };
            var assignee = t.AssigneeTypeName ?? (t.AssigneeType == AssigneeType.Mine ? "내 작업" : "타인");
            var tags = string.Join(";", t.Tags.Select(x => x.Name));
            sb.AppendLine($"\"{Esc(t.Title)}\",\"{Esc(t.ProjectName)}\",{statusLabel},{priorityLabel},\"{Esc(t.TeamName ?? "")}\",\"{Esc(assignee)}\",{t.DueDate?.ToString("yyyy-MM-dd") ?? ""},{t.StartDate?.ToString("yyyy-MM-dd") ?? ""},{t.EndDate?.ToString("yyyy-MM-dd") ?? ""},\"{Esc(tags)}\"");
        }
        // UTF-8 BOM - Excel에서 한글 깨짐 방지
        var bytes = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv; charset=utf-8", $"todos-export-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    private static string Esc(string s) => s.Replace("\"", "\"\"");
}
