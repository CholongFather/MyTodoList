using System.Net.Http.Json;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;

namespace DevTodoList.Web.Services.ApiClient;

/// <summary>버스 정류장 마스터 API 클라이언트</summary>
public class BusStationApiClient(IHttpClientFactory factory)
{
    private HttpClient Http => factory.CreateClient("DevTodoApi");

    public async Task<List<BusStationDto>> GetAllAsync()
        => await Http.GetFromJsonAsync<List<BusStationDto>>(ApiRoutes.BusStations) ?? [];

    public async Task<BusStationDto?> CreateAsync(CreateBusStationRequest req)
    {
        var resp = await Http.PostAsJsonAsync(ApiRoutes.BusStations, req);
        return await resp.Content.ReadFromJsonAsync<BusStationDto>();
    }

    public async Task<BusStationDto?> UpdateAsync(long id, CreateBusStationRequest req)
    {
        var resp = await Http.PutAsJsonAsync($"{ApiRoutes.BusStations}/{id}", req);
        return await resp.Content.ReadFromJsonAsync<BusStationDto>();
    }

    public async Task DeleteAsync(long id)
        => await Http.DeleteAsync($"{ApiRoutes.BusStations}/{id}");
}
