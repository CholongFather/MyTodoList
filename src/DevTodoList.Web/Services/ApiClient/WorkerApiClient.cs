using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

public class WorkerApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<WorkerDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<WorkerDto>>("api/workers") ?? [];

    public async Task<WorkerDto?> CreateAsync(CreateWorkerRequest req)
    {
        var res = await Http.PostAsJsonAsync("api/workers", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<WorkerDto>();
    }

    public async Task<WorkerDto?> UpdateAsync(long id, CreateWorkerRequest req)
    {
        var res = await Http.PutAsJsonAsync($"api/workers/{id}", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<WorkerDto>();
    }

    public async Task ReorderAsync(List<long> orderedIds)
    {
        var res = await Http.PutAsJsonAsync("api/workers/reorder", orderedIds);
        await res.EnsureSuccessOrThrowAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"api/workers/{id}");
        await res.EnsureSuccessOrThrowAsync();
    }
}
