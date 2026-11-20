using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

public class SettingsApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<NotificationSettingDto> GetNotificationsAsync()
        => await Http.GetFromJsonAsync<NotificationSettingDto>("api/settings/notifications") ?? new();

    public async Task UpdateNotificationsAsync(NotificationSettingDto dto)
    {
        var res = await Http.PutAsJsonAsync("api/settings/notifications", dto);
        await res.EnsureSuccessOrThrowAsync();
    }


    public async Task<List<DatabaseBackupDto>> GetBackupsAsync()
        => await Http.GetFromJsonAsync<List<DatabaseBackupDto>>("api/settings/backups") ?? [];

    public async Task<DatabaseBackupDto> CreateBackupAsync(string? description = null)
    {
        var res = await Http.PostAsJsonAsync("api/settings/backup", new CreateBackupRequest { Description = description });
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<DatabaseBackupDto>() ?? new();
    }

    public async Task DeleteBackupAsync(long id)
    {
        var res = await Http.DeleteAsync($"api/settings/backups/{id}");
        await res.EnsureSuccessOrThrowAsync();
    }

    public async Task<DatabaseBackupDto> RestoreBackupAsync(long id)
    {
        var res = await Http.PostAsync($"api/settings/backups/{id}/restore", null);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<DatabaseBackupDto>() ?? new();
    }

    public async Task<BackupSettingsDto> GetBackupSettingsAsync()
        => await Http.GetFromJsonAsync<BackupSettingsDto>("api/settings/backup-settings") ?? new();
}
