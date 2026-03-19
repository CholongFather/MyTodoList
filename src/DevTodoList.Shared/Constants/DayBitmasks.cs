namespace DevTodoList.Shared.Constants;

/// <summary>요일 비트플래그 상수</summary>
public static class DayBitmasks
{
    public const int Monday = 1;
    public const int Tuesday = 2;
    public const int Wednesday = 4;
    public const int Thursday = 8;
    public const int Friday = 16;
    public const int Saturday = 32;
    public const int Sunday = 64;

    /// <summary>평일 (월~금)</summary>
    public const int Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday; // 31

    /// <summary>주말 (토~일)</summary>
    public const int Weekend = Saturday | Sunday; // 96

    /// <summary>매일</summary>
    public const int EveryDay = Weekdays | Weekend; // 127

    public static int GetDayBit(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => Monday,
        DayOfWeek.Tuesday => Tuesday,
        DayOfWeek.Wednesday => Wednesday,
        DayOfWeek.Thursday => Thursday,
        DayOfWeek.Friday => Friday,
        DayOfWeek.Saturday => Saturday,
        DayOfWeek.Sunday => Sunday,
        _ => 0
    };

    /// <summary>비트플래그 → 한글 요일 텍스트</summary>
    public static string ToKoreanText(int days)
    {
        if (days == Weekdays) return "평일 (월~금)";
        if (days == EveryDay) return "매일";
        if (days == Weekend) return "주말 (토~일)";

        var labels = new List<string>();
        ReadOnlySpan<(int flag, string label)> options =
        [
            (Monday, "월"), (Tuesday, "화"), (Wednesday, "수"), (Thursday, "목"),
            (Friday, "금"), (Saturday, "토"), (Sunday, "일")
        ];
        foreach (var (flag, label) in options)
        {
            if ((days & flag) != 0) labels.Add(label);
        }
        return labels.Count > 0 ? string.Join(", ", labels) : "없음";
    }
}
