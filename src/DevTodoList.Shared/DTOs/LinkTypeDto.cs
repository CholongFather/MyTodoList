namespace DevTodoList.Shared.DTOs;

/// <summary>링크 유형 DTO</summary>
public class LinkTypeDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public string? Icon { get; set; }
    /// <summary>URL 자동감지 패턴 (쉼표 구분)</summary>
    public string? UrlPattern { get; set; }
    public int SortOrder { get; set; }
}
