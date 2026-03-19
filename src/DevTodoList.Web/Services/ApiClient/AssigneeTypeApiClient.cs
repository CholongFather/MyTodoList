using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

/// <summary>담당 유형 API 클라이언트</summary>
public class AssigneeTypeApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<AssigneeTypeDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<AssigneeTypeDto>>("api/assignee-types") ?? [];

    public async Task<AssigneeTypeDto?> CreateAsync(CreateAssigneeTypeRequest req)
    {
        var resp = await Http.PostAsJsonAsync("api/assignee-types", req);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<AssigneeTypeDto>();
    }

    public async Task<AssigneeTypeDto?> UpdateAsync(long id, CreateAssigneeTypeRequest req)
    {
        var resp = await Http.PutAsJsonAsync($"api/assignee-types/{id}", req);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<AssigneeTypeDto>();
    }

    public async Task ReorderAsync(List<long> orderedIds)
    {
        var resp = await Http.PutAsJsonAsync("api/assignee-types/reorder", new ReorderRequest { OrderedIds = orderedIds });
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(long id)
    {
        var resp = await Http.DeleteAsync($"api/assignee-types/{id}");
        resp.EnsureSuccessStatusCode();
    }
}
