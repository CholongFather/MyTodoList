using System.Net.Http.Json;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;

namespace DevTodoList.Web.Services.ApiClient;

public class TodoApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<TodoItemDto>> GetAllAsync(
        long? projectId = null, TodoStatus? status = null, string? search = null,
        long? teamId = null, AssigneeType? assigneeType = null, long? workCategoryId = null,
        long? assigneeTypeId = null)
    {
        var query = new List<string>();
        if (projectId.HasValue) query.Add($"projectId={projectId}");
        if (status.HasValue) query.Add($"status={(int)status}");
        if (!string.IsNullOrEmpty(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (teamId.HasValue) query.Add($"teamId={teamId}");
        if (assigneeType.HasValue) query.Add($"assigneeType={(int)assigneeType}");
        if (workCategoryId.HasValue) query.Add($"workCategoryId={workCategoryId}");
        if (assigneeTypeId.HasValue) query.Add($"assigneeTypeId={assigneeTypeId}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : "";
        return await Http.GetFromJsonAsync<List<TodoItemDto>>($"api/todos{qs}") ?? [];
    }

    /// <summary>페이지네이션 조회</summary>
    public async Task<PagedResult<TodoItemDto>> GetPagedAsync(
        long? projectId = null, TodoStatus? status = null, string? search = null,
        long? teamId = null, AssigneeType? assigneeType = null, long? workCategoryId = null,
        long? assigneeTypeId = null,
        int page = PaginationDefaults.Page, int pageSize = PaginationDefaults.PageSize)
    {
        var query = new List<string> { $"page={page}", $"pageSize={pageSize}" };
        if (projectId.HasValue) query.Add($"projectId={projectId}");
        if (status.HasValue) query.Add($"status={(int)status}");
        if (!string.IsNullOrEmpty(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (teamId.HasValue) query.Add($"teamId={teamId}");
        if (assigneeType.HasValue) query.Add($"assigneeType={(int)assigneeType}");
        if (workCategoryId.HasValue) query.Add($"workCategoryId={workCategoryId}");
        if (assigneeTypeId.HasValue) query.Add($"assigneeTypeId={assigneeTypeId}");
        var qs = "?" + string.Join("&", query);
        return await Http.GetFromJsonAsync<PagedResult<TodoItemDto>>($"api/todos/paged{qs}") ?? new();
    }

    public async Task<TodoItemDto?> GetByIdAsync(long id)
        => await Http.GetFromJsonAsync<TodoItemDto>($"api/todos/{id}");

    public async Task<TodoItemDto?> CreateAsync(CreateTodoRequest req)
    {
        var res = await Http.PostAsJsonAsync("api/todos", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<TodoItemDto>();
    }

    public async Task<TodoItemDto?> UpdateAsync(long id, UpdateTodoRequest req)
    {
        var res = await Http.PutAsJsonAsync($"api/todos/{id}", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<TodoItemDto>();
    }

    public async Task UpdateStatusAsync(long id, TodoStatus status)
    {
        var res = await Http.PatchAsJsonAsync($"api/todos/{id}/status", new UpdateStatusRequest { Status = status });
        await res.EnsureSuccessOrThrowAsync();
    }

    /// <summary>태그 즉시 동기화</summary>
    public async Task UpdateTagsAsync(long id, List<long> tagIds)
    {
        var res = await Http.PatchAsJsonAsync($"api/todos/{id}/tags", new UpdateTagsRequest { TagIds = tagIds });
        await res.EnsureSuccessOrThrowAsync();
    }

    /// <summary>작업자 즉시 동기화</summary>
    public async Task UpdateWorkersAsync(long id, List<long> workerIds)
    {
        var res = await Http.PatchAsJsonAsync($"api/todos/{id}/workers", new UpdateWorkersRequest { WorkerIds = workerIds });
        await res.EnsureSuccessOrThrowAsync();
    }

    /// <summary>담당 유형 즉시 변경</summary>
    public async Task UpdateAssigneeTypeAsync(long id, long? assigneeTypeId)
    {
        var res = await Http.PatchAsJsonAsync($"api/todos/{id}/assignee-type", new UpdateAssigneeTypeRequest { AssigneeTypeId = assigneeTypeId });
        await res.EnsureSuccessOrThrowAsync();
    }

    /// <summary>순서 변경</summary>
    public async Task ReorderAsync(List<long> orderedIds)
    {
        var res = await Http.PutAsJsonAsync("api/todos/reorder", new ReorderRequest { OrderedIds = orderedIds });
        await res.EnsureSuccessOrThrowAsync();
    }

    public async Task<TodoItemDto?> DuplicateAsync(long id)
    {
        var res = await Http.PostAsync($"api/todos/{id}/duplicate", null);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<TodoItemDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"api/todos/{id}");
        await res.EnsureSuccessOrThrowAsync();
    }

    // 체크리스트
    public async Task<TodoCheckItemDto?> CreateCheckItemAsync(long todoId, string title)
    {
        var res = await Http.PostAsJsonAsync($"api/todos/{todoId}/checkitems", new CreateCheckItemRequest { Title = title });
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<TodoCheckItemDto>();
    }

    public async Task ToggleCheckItemAsync(long todoId, long checkItemId)
    {
        var res = await Http.PatchAsync($"api/todos/{todoId}/checkitems/{checkItemId}/toggle", null);
        await res.EnsureSuccessOrThrowAsync();
    }

    public async Task DeleteCheckItemAsync(long todoId, long checkItemId)
    {
        var res = await Http.DeleteAsync($"api/todos/{todoId}/checkitems/{checkItemId}");
        await res.EnsureSuccessOrThrowAsync();
    }

    // 링크
    public async Task<TodoLinkDto?> CreateLinkAsync(long todoId, CreateLinkRequest req)
    {
        var res = await Http.PostAsJsonAsync($"api/todos/{todoId}/links", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<TodoLinkDto>();
    }

    public async Task DeleteLinkAsync(long todoId, long linkId)
    {
        var res = await Http.DeleteAsync($"api/todos/{todoId}/links/{linkId}");
        await res.EnsureSuccessOrThrowAsync();
    }

    /// <summary>CSV 내보내기</summary>
    public async Task<byte[]> ExportCsvAsync(long? projectId = null, int? status = null, long? teamId = null, int? assigneeType = null)
    {
        var query = new List<string>();
        if (projectId.HasValue) query.Add($"projectId={projectId}");
        if (status.HasValue) query.Add($"status={status}");
        if (teamId.HasValue) query.Add($"teamId={teamId}");
        if (assigneeType.HasValue) query.Add($"assigneeType={assigneeType}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : "";
        return await Http.GetByteArrayAsync($"api/todos/export/csv{qs}");
    }
}
