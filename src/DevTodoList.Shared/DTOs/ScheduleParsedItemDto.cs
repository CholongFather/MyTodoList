namespace DevTodoList.Shared.DTOs;

/// <summary>일정 파싱 결과 항목</summary>
public class ScheduleParsedItemDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Depth { get; set; }
    public string? ParentTitle { get; set; }
    /// <summary>마일스톤 여부 (단일 날짜)</summary>
    public bool IsMilestone { get; set; }

    // 팀/담당자 (개별 항목 오버라이드용)
    public long? TeamId { get; set; }
    public long? WorkCategoryId { get; set; }
}
