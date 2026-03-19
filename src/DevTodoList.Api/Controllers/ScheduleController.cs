using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController(ScheduleParseService svc) : ControllerBase
{
    /// <summary>헬스체크용</summary>
    [HttpGet]
    public IActionResult Health() => Ok(new { status = "ok" });

    /// <summary>일정 텍스트 파싱 (미리보기)</summary>
    [HttpPost("parse")]
    public IActionResult Parse([FromBody] ScheduleParseRequest req, CancellationToken ct)
        => Ok(svc.Parse(req.Text));

    /// <summary>파싱된 일정을 TODO로 일괄 생성</summary>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ScheduleParseRequest req, CancellationToken ct)
    {
        var parsed = svc.Parse(req.Text);
        if (parsed.Count == 0) return BadRequest("파싱 결과가 없습니다.");
        var result = await svc.CreateFromParsedAsync(parsed, req.ProjectId,
            req.DefaultTeamId, req.DefaultWorkCategoryId, (int)req.DefaultAssigneeType,
            req.DefaultAssigneeTypeId, req.DefaultIsExternal, ct);
        return Ok(result);
    }

    /// <summary>프로젝트의 기존 TODO를 삭제하고 새 일정으로 일괄 교체</summary>
    [HttpPut("replace")]
    public async Task<IActionResult> Replace([FromBody] ScheduleParseRequest req, CancellationToken ct)
    {
        if (req.ProjectId <= 0) return BadRequest("프로젝트 ID가 필요합니다.");
        var result = await svc.ReplaceFromTextAsync(req.Text, req.ProjectId,
            req.DefaultTeamId, req.DefaultWorkCategoryId, (int)req.DefaultAssigneeType,
            req.DefaultAssigneeTypeId, req.DefaultIsExternal, ct);
        return Ok(result);
    }
}
