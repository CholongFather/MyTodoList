using System.Net.Http.Json;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;

namespace DevTodoList.Web.Services.ApiClient;

/// <summary>케이스 트래커 API 클라이언트</summary>
public class CaseApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");
    private const string Base = "api/cases";

    public async Task<List<CaseDto>> GetAllAsync(
        CaseStatus? status = null, CaseCategory? category = null,
        long? environmentId = null, long? projectId = null,
        string? search = null)
    {
        var query = new List<string>();
        if (status.HasValue) query.Add($"status={status}");
        if (category.HasValue) query.Add($"category={category}");
        if (environmentId.HasValue) query.Add($"environmentId={environmentId}");
        if (projectId.HasValue) query.Add($"projectId={projectId}");
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");

        var url = query.Count > 0 ? $"{Base}?{string.Join("&", query)}" : Base;
        return await Http.GetFromJsonAsync<List<CaseDto>>(url) ?? [];
    }

    /// <summary>페이지네이션 조회</summary>
    public async Task<PagedResult<CaseDto>> GetPagedAsync(
        CaseStatus? status = null, CaseCategory? category = null,
        long? environmentId = null, long? projectId = null,
        string? search = null, int page = PaginationDefaults.Page, int pageSize = PaginationDefaults.PageSize)
    {
        var query = new List<string> { $"page={page}", $"pageSize={pageSize}" };
        if (status.HasValue) query.Add($"status={status}");
        if (category.HasValue) query.Add($"category={category}");
        if (environmentId.HasValue) query.Add($"environmentId={environmentId}");
        if (projectId.HasValue) query.Add($"projectId={projectId}");
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        var qs = "?" + string.Join("&", query);
        return await Http.GetFromJsonAsync<PagedResult<CaseDto>>($"{Base}/paged{qs}") ?? new();
    }

    public async Task<CaseDto?> GetByIdAsync(long id)
        => await Http.GetFromJsonAsync<CaseDto>($"{Base}/{id}");

    public async Task<CaseDto?> CreateAsync(CreateCaseRequest req)
    {
        var res = await Http.PostAsJsonAsync(Base, req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CaseDto>();
    }

    public async Task<CaseDto?> UpdateAsync(long id, CreateCaseRequest req)
    {
        var res = await Http.PutAsJsonAsync($"{Base}/{id}", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CaseDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"{Base}/{id}");
        res.EnsureSuccessStatusCode();
    }

    public async Task<CaseDto?> UpdateStatusAsync(long id, UpdateCaseStatusRequest req)
    {
        var res = await Http.PutAsJsonAsync($"{Base}/{id}/status", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CaseDto>();
    }

    public async Task<object?> GetStatsAsync()
        => await Http.GetFromJsonAsync<object>($"{Base}/stats");

    // === 노트 ===

    public async Task<CaseNoteDto?> CreateNoteAsync(long caseId, CreateCaseNoteRequest req)
    {
        var res = await Http.PostAsJsonAsync($"{Base}/{caseId}/notes", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CaseNoteDto>();
    }

    public async Task<CaseNoteDto?> UpdateNoteAsync(long caseId, long noteId, CreateCaseNoteRequest req)
    {
        var res = await Http.PutAsJsonAsync($"{Base}/{caseId}/notes/{noteId}", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CaseNoteDto>();
    }

    public async Task DeleteNoteAsync(long caseId, long noteId)
    {
        var res = await Http.DeleteAsync($"{Base}/{caseId}/notes/{noteId}");
        res.EnsureSuccessStatusCode();
    }

    // === 링크 ===

    public async Task<CaseLinkDto?> CreateLinkAsync(long caseId, CreateLinkRequest req)
    {
        var res = await Http.PostAsJsonAsync($"{Base}/{caseId}/links", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CaseLinkDto>();
    }

    public async Task<CaseLinkDto?> UpdateLinkAsync(long caseId, long linkId, CreateLinkRequest req)
    {
        var res = await Http.PutAsJsonAsync($"{Base}/{caseId}/links/{linkId}", req);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<CaseLinkDto>();
    }

    public async Task DeleteLinkAsync(long caseId, long linkId)
    {
        var res = await Http.DeleteAsync($"{Base}/{caseId}/links/{linkId}");
        res.EnsureSuccessStatusCode();
    }
}
