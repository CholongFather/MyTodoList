using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

public class ProjectApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<ProjectDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<ProjectDto>>("api/projects") ?? [];

    public async Task<ProjectDto?> GetByIdAsync(long id)
        => await Http.GetFromJsonAsync<ProjectDto>($"api/projects/{id}");

    public async Task<ProjectDto?> CreateAsync(CreateProjectRequest req)
    {
        var res = await Http.PostAsJsonAsync("api/projects", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<ProjectDto>();
    }

    public async Task<ProjectDto?> UpdateAsync(long id, CreateProjectRequest req)
    {
        var res = await Http.PutAsJsonAsync($"api/projects/{id}", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<ProjectDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"api/projects/{id}");
        await res.EnsureSuccessOrThrowAsync();
    }

    // === 메타 (URL/버전/비밀번호) ===

    public async Task<ProjectMetaDto?> CreateMetaAsync(long projectId, CreateProjectMetaRequest req)
    {
        var res = await Http.PostAsJsonAsync($"api/projects/{projectId}/metas", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<ProjectMetaDto>();
    }

    public async Task<ProjectMetaDto?> UpdateMetaAsync(long metaId, CreateProjectMetaRequest req)
    {
        var res = await Http.PutAsJsonAsync($"api/projects/metas/{metaId}", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<ProjectMetaDto>();
    }

    public async Task DeleteMetaAsync(long metaId)
    {
        var res = await Http.DeleteAsync($"api/projects/metas/{metaId}");
        await res.EnsureSuccessOrThrowAsync();
    }
}
