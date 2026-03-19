namespace DevTodoList.Shared.Enums;

/// <summary>버스 진행 방향</summary>
public enum BusDirection
{
    Unspecified = 0,  // 미지정
    Upbound = 1,      // 상행
    Downbound = 2     // 하행
}

public static class BusDirectionExtensions
{
    public static string ToKorean(this BusDirection direction) => direction switch
    {
        BusDirection.Upbound => "상행",
        BusDirection.Downbound => "하행",
        _ => ""
    };
}
