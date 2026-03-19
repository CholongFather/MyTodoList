using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

public class LinkTypeApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<LinkTypeDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<LinkTypeDto>>("api/link-types") ?? [];

    public async Task<LinkTypeDto?> CreateAsync(CreateLinkTypeRequest req)
    {
        var res = await Http.PostAsJsonAsync("api/link-types", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<LinkTypeDto>();
    }

    public async Task<LinkTypeDto?> UpdateAsync(long id, CreateLinkTypeRequest req)
    {
        var res = await Http.PutAsJsonAsync($"api/link-types/{id}", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<LinkTypeDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"api/link-types/{id}");
        await res.EnsureSuccessOrThrowAsync();
    }
}
