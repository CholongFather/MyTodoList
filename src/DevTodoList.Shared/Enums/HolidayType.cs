namespace DevTodoList.Shared.Enums;

/// <summary>휴일 유형</summary>
public enum HolidayType
{
    /// <summary>공휴일</summary>
    PublicHoliday = 0,
    /// <summary>개인 연차</summary>
    PersonalLeave = 1
}

public static class HolidayTypeExtensions
{
    public static string ToKorean(this HolidayType h) => h switch
    {
        HolidayType.PublicHoliday => "공휴일",
        HolidayType.PersonalLeave => "연차",
        _ => h.ToString()
    };
}
