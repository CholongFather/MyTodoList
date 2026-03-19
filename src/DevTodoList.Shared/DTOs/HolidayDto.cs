using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

/// <summary>공휴일/연차 DTO</summary>
public class HolidayDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public HolidayType HolidayType { get; set; }
}
