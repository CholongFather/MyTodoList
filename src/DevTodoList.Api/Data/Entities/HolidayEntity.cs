using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>공휴일/휴일 엔티티</summary>
[Table("Holidays")]
public class HolidayEntity : EntityBase
{
    /// <summary>휴일 날짜</summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>휴일 이름 (예: 설날, 추석)</summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>매년 반복 여부 (고정 공휴일)</summary>
    public bool IsRecurring { get; set; }

    /// <summary>휴일 유형: PublicHoliday(0), PersonalLeave(1)</summary>
    public int HolidayType { get; set; }
}
