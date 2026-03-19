using System.Net.Http.Json;
using DevTodoList.Shared.DTOs;

namespace DevTodoList.Web.Services.ApiClient;

public class DashboardApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<DashboardDto> GetAsync()
        => await Http.GetFromJsonAsync<DashboardDto>("api/dashboard") ?? new();
}
