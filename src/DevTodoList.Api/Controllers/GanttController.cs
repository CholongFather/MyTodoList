using DevTodoList.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GanttController(GanttService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] long? projectId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] long? teamId,
        [FromQuery] int? assigneeType,
        CancellationToken ct)
        => Ok(await svc.GetAsync(projectId, fromDate, toDate, teamId, assigneeType, ct));
}
