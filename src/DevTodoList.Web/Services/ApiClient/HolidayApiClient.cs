using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

/// <summary>공휴일 API 클라이언트</summary>
public class HolidayApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<HolidayDto>> GetByYearAsync(int year)
        => await Http.GetFromJsonAsync<List<HolidayDto>>($"api/holidays?year={year}") ?? [];

    public async Task<List<HolidayDto>> GetByRangeAsync(DateTime from, DateTime to)
        => await Http.GetFromJsonAsync<List<HolidayDto>>($"api/holidays/range?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}") ?? [];

    public async Task<HolidayDto?> CreateAsync(CreateHolidayRequest req)
    {
        var resp = await Http.PostAsJsonAsync("api/holidays", req);
        await resp.EnsureSuccessOrThrowAsync();
        return await resp.Content.ReadFromJsonAsync<HolidayDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var resp = await Http.DeleteAsync($"api/holidays/{id}");
        await resp.EnsureSuccessOrThrowAsync();
    }

    public async Task<int> SeedKoreanAsync(int year)
    {
        var resp = await Http.PostAsync($"api/holidays/seed/{year}", null);
        await resp.EnsureSuccessOrThrowAsync();
        var result = await resp.Content.ReadFromJsonAsync<SeedResult>();
        return result?.Added ?? 0;
    }

    private record SeedResult(int Added);
}
