using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>케이스 관련 링크</summary>
[Table("CaseLinks")]
public class CaseLinkEntity : EntityBase
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Url { get; set; } = string.Empty;

    /// <summary>링크 유형: Jira=0, Confluence=1, GitHub=2, GitLab=3, Custom=4 - 레거시</summary>
    public int LinkType { get; set; } = 4;

    /// <summary>링크 유형 FK (DB 관리)</summary>
    public long? LinkTypeId { get; set; }

    public long CaseId { get; set; }

    [ForeignKey(nameof(CaseId))]
    public CaseEntity Case { get; set; } = null!;

    [ForeignKey(nameof(LinkTypeId))]
    public LinkTypeEntity? LinkTypeEntity { get; set; }
}
