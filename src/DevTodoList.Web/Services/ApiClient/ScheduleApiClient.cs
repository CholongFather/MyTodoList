using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;

namespace DevTodoList.Web.Services.ApiClient;

public class ScheduleApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<ScheduleParsedItemDto>> ParseAsync(string text)
    {
        var res = await Http.PostAsJsonAsync("api/schedule/parse", new ScheduleParseRequest { Text = text });
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<List<ScheduleParsedItemDto>>() ?? [];
    }

    public async Task<List<TodoItemDto>> CreateAsync(string text, long projectId,
        long? defaultTeamId = null, long? defaultWorkCategoryId = null,
        AssigneeType defaultAssigneeType = AssigneeType.Mine,
        long? defaultAssigneeTypeId = null, bool defaultIsExternal = false)
    {
        var req = new ScheduleParseRequest
        {
            Text = text,
            ProjectId = projectId,
            DefaultTeamId = defaultTeamId,
            DefaultWorkCategoryId = defaultWorkCategoryId,
            DefaultAssigneeType = defaultAssigneeType,
            DefaultAssigneeTypeId = defaultAssigneeTypeId,
            DefaultIsExternal = defaultIsExternal
        };
        var res = await Http.PostAsJsonAsync("api/schedule/create", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<List<TodoItemDto>>() ?? [];
    }

    public async Task<BulkReplaceResultDto?> ReplaceAsync(string text, long projectId,
        long? defaultTeamId = null, long? defaultWorkCategoryId = null,
        AssigneeType defaultAssigneeType = AssigneeType.Mine,
        long? defaultAssigneeTypeId = null, bool defaultIsExternal = false)
    {
        var req = new ScheduleParseRequest
        {
            Text = text,
            ProjectId = projectId,
            DefaultTeamId = defaultTeamId,
            DefaultWorkCategoryId = defaultWorkCategoryId,
            DefaultAssigneeType = defaultAssigneeType,
            DefaultAssigneeTypeId = defaultAssigneeTypeId,
            DefaultIsExternal = defaultIsExternal
        };
        var res = await Http.PutAsJsonAsync("api/schedule/replace", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<BulkReplaceResultDto>();
    }
}
