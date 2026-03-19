namespace DevTodoList.Shared.Constants;

/// <summary>간트차트 레이아웃 상수</summary>
public static class GanttDefaults
{
    // 일자별 픽셀 폭
    public const int InitialDayWidth = 40;
    public const int MinDayWidth = 20;
    public const int MaxDayWidth = 80;
    public const int ZoomStep = 8;

    // 날짜 범위 여백 (일)
    public const int MarginBefore = 3;
    public const int MarginAfter = 7;
    public const int EmptyViewBefore = 7;
    public const int EmptyViewAfter = 30;

    // 행 높이 (px)
    public const int DualBarRowHeight = 52;
    public const int SingleBarRowHeight = 40;
    public const int HeaderRowHeight = 36;

    // 바 높이 (px)
    public const int PlannedBarHeight = 18;
    public const int ActualBarHeight = 20;
    public const int SingleBarHeight = 28;

    // 왼쪽 작업명 컬럼 폭 (px)
    public const int LeftColumnWidth = 240;
}
