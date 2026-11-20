using System.Net.Http.Json;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

/// <summary>팀/작업분류 API 클라이언트</summary>
public class TeamApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<TeamDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<TeamDto>>(ApiRoutes.Teams) ?? [];

    public async Task<TeamDto?> GetByIdAsync(long id)
        => await Http.GetFromJsonAsync<TeamDto>($"{ApiRoutes.Teams}/{id}");

    public async Task<TeamDto?> CreateAsync(CreateTeamRequest req)
    {
        var resp = await Http.PostAsJsonAsync(ApiRoutes.Teams, req);
        await resp.EnsureSuccessOrThrowAsync();
        return await resp.Content.ReadFromJsonAsync<TeamDto>();
    }

    public async Task<TeamDto?> UpdateAsync(long id, CreateTeamRequest req)
    {
        var resp = await Http.PutAsJsonAsync($"{ApiRoutes.Teams}/{id}", req);
        await resp.EnsureSuccessOrThrowAsync();
        return await resp.Content.ReadFromJsonAsync<TeamDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var resp = await Http.DeleteAsync($"{ApiRoutes.Teams}/{id}");
        await resp.EnsureSuccessOrThrowAsync();
    }

    // === 작업 분류 ===

    public async Task<WorkCategoryDto?> CreateCategoryAsync(long teamId, CreateWorkCategoryRequest req)
    {
        var resp = await Http.PostAsJsonAsync($"{ApiRoutes.Teams}/{teamId}/categories", req);
        await resp.EnsureSuccessOrThrowAsync();
        return await resp.Content.ReadFromJsonAsync<WorkCategoryDto>();
    }

    public async Task<WorkCategoryDto?> UpdateCategoryAsync(long id, CreateWorkCategoryRequest req)
    {
        var resp = await Http.PutAsJsonAsync($"{ApiRoutes.Teams}/categories/{id}", req);
        await resp.EnsureSuccessOrThrowAsync();
        return await resp.Content.ReadFromJsonAsync<WorkCategoryDto>();
    }

    public async Task DeleteCategoryAsync(long id)
    {
        var resp = await Http.DeleteAsync($"{ApiRoutes.Teams}/categories/{id}");
        await resp.EnsureSuccessOrThrowAsync();
    }
}
