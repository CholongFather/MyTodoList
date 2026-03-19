using System.Net.Http.Json;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

/// <summary>버스 출퇴근 알림 API 클라이언트</summary>
public class BusCommuteApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");
    private const string Base = ApiRoutes.BusCommute;

    public async Task<List<BusCommuteSettingDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<BusCommuteSettingDto>>(Base) ?? [];

    public async Task<BusCommuteSettingDto?> GetByIdAsync(long id)
        => await Http.GetFromJsonAsync<BusCommuteSettingDto>($"{Base}/{id}");

    public async Task<BusCommuteSettingDto> CreateAsync(CreateBusCommuteSettingRequest req)
    {
        var res = await Http.PostAsJsonAsync(Base, req);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<BusCommuteSettingDto>())!;
    }

    public async Task<BusCommuteSettingDto> UpdateAsync(long id, CreateBusCommuteSettingRequest req)
    {
        var res = await Http.PutAsJsonAsync($"{Base}/{id}", req);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<BusCommuteSettingDto>())!;
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"{Base}/{id}");
        res.EnsureSuccessStatusCode();
    }

    public async Task ToggleAsync(long id)
    {
        var res = await Http.PutAsync($"{Base}/{id}/toggle", null);
        res.EnsureSuccessStatusCode();
    }

    public async Task<bool> TestNotificationAsync(long settingId)
    {
        var res = await Http.PostAsJsonAsync($"{Base}/test-notification", new TestNotificationRequest { SettingId = settingId });
        return res.IsSuccessStatusCode;
    }

    /// <summary>특정 설정의 실시간 도착정보 조회</summary>
    public async Task<BusArrivalInfoDto?> GetArrivalAsync(long settingId)
    {
        try { return await Http.GetFromJsonAsync<BusArrivalInfoDto>($"{Base}/{settingId}/arrival"); }
        catch { return null; }
    }
}
