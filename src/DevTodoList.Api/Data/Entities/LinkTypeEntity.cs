using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>링크 유형 마스터</summary>
[Table("LinkTypes")]
public class LinkTypeEntity : EntityBase
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10)]
    public string Color { get; set; } = "#9E9E9E";

    /// <summary>Material Icon 이름 (예: BugReport, MenuBook, Code, Link)</summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>URL 자동감지 패턴 (쉼표 구분, 예: "jira,atlassian.net/browse")</summary>
    [MaxLength(500)]
    public string? UrlPattern { get; set; }

    public int SortOrder { get; set; }
}
