using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

/// <summary>배포 환경 API 클라이언트</summary>
public class EnvironmentApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<EnvironmentDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<EnvironmentDto>>("api/environments") ?? [];

    public async Task<EnvironmentDto?> CreateAsync(CreateEnvironmentRequest req)
    {
        var resp = await Http.PostAsJsonAsync("api/environments", req);
        return await resp.Content.ReadFromJsonAsync<EnvironmentDto>();
    }

    public async Task DeleteAsync(long id)
        => await Http.DeleteAsync($"api/environments/{id}");
}
