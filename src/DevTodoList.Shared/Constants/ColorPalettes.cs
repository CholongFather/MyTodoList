namespace DevTodoList.Shared.Constants;

/// <summary>설정 페이지 등에서 사용하는 색상 팔레트</summary>
public static class ColorPalettes
{
    /// <summary>기본 15색 팔레트 (작업자/노트유형/링크유형)</summary>
    public static readonly string[] Standard =
    [
        "#E91E63", "#9C27B0", "#673AB7", "#3F51B5", "#2196F3",
        "#00BCD4", "#009688", "#4CAF50", "#8BC34A", "#FF9800",
        "#FF5722", "#795548", "#607D8B", "#F44336", "#1976D2"
    ];

    /// <summary>확장 20색 팔레트 (태그)</summary>
    public static readonly string[] Extended =
    [
        "#F44336", "#E91E63", "#9C27B0", "#673AB7", "#3F51B5",
        "#2196F3", "#03A9F4", "#00BCD4", "#009688", "#4CAF50",
        "#8BC34A", "#CDDC39", "#FFC107", "#FF9800", "#FF5722",
        "#795548", "#607D8B", "#9E9E9E", "#000000", "#37474F"
    ];

    /// <summary>팔레트에서 랜덤 색상 반환</summary>
    public static string Random(string[]? palette = null)
        => (palette ?? Standard)[System.Random.Shared.Next((palette ?? Standard).Length)];
}
