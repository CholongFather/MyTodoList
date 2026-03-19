namespace DevTodoList.Web.Services.ApiClient;

/// <summary>HttpResponseMessage 확장 메서드</summary>
public static class HttpResponseExtensions
{
    /// <summary>응답 실패 시 본문 포함 에러 throw</summary>
    public static async Task EnsureSuccessOrThrowAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {body}");
    }
}
