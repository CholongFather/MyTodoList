using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.Enums;

namespace DevTodoList.Web.Services.ApiClient;

public class GanttApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<GanttItemDto>> GetAsync(
        long? projectId = null, DateTime? fromDate = null, DateTime? toDate = null,
        long? teamId = null, AssigneeType? assigneeType = null)
    {
        var query = new List<string>();
        if (projectId.HasValue) query.Add($"projectId={projectId}");
        if (fromDate.HasValue) query.Add($"fromDate={fromDate:yyyy-MM-dd}");
        if (toDate.HasValue) query.Add($"toDate={toDate:yyyy-MM-dd}");
        if (teamId.HasValue) query.Add($"teamId={teamId}");
        if (assigneeType.HasValue) query.Add($"assigneeType={(int)assigneeType}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : "";
        return await Http.GetFromJsonAsync<List<GanttItemDto>>($"api/gantt{qs}") ?? [];
    }
}
