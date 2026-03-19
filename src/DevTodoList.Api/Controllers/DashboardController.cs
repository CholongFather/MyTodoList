using DevTodoList.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(DashboardService svc) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await svc.GetAsync(ct));
}
