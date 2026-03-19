using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

public class NoteTypeApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<NoteTypeDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<NoteTypeDto>>("api/note-types") ?? [];

    public async Task<NoteTypeDto?> CreateAsync(CreateNoteTypeRequest req)
    {
        var res = await Http.PostAsJsonAsync("api/note-types", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<NoteTypeDto>();
    }

    public async Task<NoteTypeDto?> UpdateAsync(long id, CreateNoteTypeRequest req)
    {
        var res = await Http.PutAsJsonAsync($"api/note-types/{id}", req);
        await res.EnsureSuccessOrThrowAsync();
        return await res.Content.ReadFromJsonAsync<NoteTypeDto>();
    }

    public async Task DeleteAsync(long id)
    {
        var res = await Http.DeleteAsync($"api/note-types/{id}");
        await res.EnsureSuccessOrThrowAsync();
    }
}
