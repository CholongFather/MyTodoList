using DevTodoList.Api.Services;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace DevTodoList.Api.Controllers;

[ApiController]
[Route("api/holidays")]
public class HolidaysController(HolidayService svc) : ControllerBase
{
    /// <summary>연도별 공휴일 목록</summary>
    [HttpGet]
    public async Task<IActionResult> GetByYear([FromQuery] int? year, CancellationToken ct)
        => Ok(await svc.GetByYearAsync(year ?? DateTime.UtcNow.Year, ct));

    /// <summary>기간별 공휴일 (간트차트용)</summary>
    [HttpGet("range")]
    public async Task<IActionResult> GetByRange([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
        => Ok(await svc.GetByRangeAsync(from, to, ct));

    /// <summary>공휴일 추가</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHolidayRequest req, CancellationToken ct)
        => Ok(await svc.CreateAsync(req, ct));

    /// <summary>공휴일 삭제</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
        => await svc.DeleteAsync(id, ct) ? NoContent() : NotFound();

    /// <summary>한국 공휴일 일괄 등록</summary>
    [HttpPost("seed/{year:int}")]
    public async Task<IActionResult> SeedKorean(int year, CancellationToken ct)
    {
        var count = await svc.SeedKoreanHolidaysAsync(year, ct);
        return Ok(new { added = count });
    }
}
