namespace DevTodoList.Web;

/// <summary>빌드 정보 (DLL 최종 빌드 시간 기반)</summary>
public static class BuildInfo
{
    public static string Version { get; } =
        File.GetLastWriteTime(typeof(BuildInfo).Assembly.Location).ToString("yyyy.MM.dd.HHmm");
}
