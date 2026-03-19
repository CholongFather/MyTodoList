using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;

namespace DevTodoList.Web.Services.ApiClient;

public class SettingsApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<NotificationSettingDto> GetNotificationsAsync()
        => await Http.GetFromJsonAsync<NotificationSettingDto>("api/settings/notifications") ?? new();

    public async Task UpdateNotificationsAsync(NotificationSettingDto dto)
    {
        var res = await Http.PutAsJsonAsync("api/settings/notifications", dto);
        res.EnsureSuccessStatusCode();
    }
}
