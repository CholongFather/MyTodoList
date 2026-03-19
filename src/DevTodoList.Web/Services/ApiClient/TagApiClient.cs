using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

public class TagApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<TagDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<TagDto>>("api/tags") ?? [];

    public async Task<TagDto?> CreateAsync(CreateTagRequest req)
    {
        var res = await Http.PostAsJsonAsync("api/tags", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<TagDto>();
    }

    public async Task<TagDto?> UpdateAsync(long id, CreateTagRequest req)
    {
        var res = await Http.PutAsJsonAsync($"api/tags/{id}", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<TagDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"api/tags/{id}");
        await res.EnsureSuccessOrThrowAsync();
    }
}
