using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>Dev/QA 케이스 (이슈 추적)</summary>
[Table("Cases")]
public class CaseEntity : EntityBase
{
    /// <summary>케이스 제목</summary>
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>상세 설명</summary>
    public string? Description { get; set; }

    /// <summary>상태: Open(0), InProgress(1), Resolved(2), Closed(3)</summary>
    public int CaseStatus { get; set; }

    /// <summary>카테고리: Bug(0), Enhancement(1), Question(2), Task(3)</summary>
    public int CaseCategory { get; set; }

    /// <summary>환경 (레거시 enum - 더 이상 사용 안 함)</summary>
    public int Environment { get; set; }

    /// <summary>환경 (DB 기반)</summary>
    public long? EnvironmentId { get; set; }

    [ForeignKey(nameof(EnvironmentId))]
    public EnvironmentEntity? EnvironmentEntity { get; set; }

    /// <summary>우선순위: Low(0), Normal(1), High(2), Critical(3)</summary>
    public int Priority { get; set; }

    /// <summary>보고자</summary>
    [MaxLength(100)]
    public string? Reporter { get; set; }

    /// <summary>담당자</summary>
    [MaxLength(100)]
    public string? Assignee { get; set; }

    /// <summary>Jira 이슈 URL</summary>
    [MaxLength(500)]
    public string? JiraUrl { get; set; }

    /// <summary>Wide 배포 URL</summary>
    [MaxLength(500)]
    public string? WideUrl { get; set; }

    /// <summary>연결된 프로젝트 (optional)</summary>
    public long? ProjectId { get; set; }

    /// <summary>해결 시각</summary>
    public DateTime? ResolvedAt { get; set; }

    // Navigation
    [ForeignKey(nameof(ProjectId))]
    public ProjectEntity? Project { get; set; }

    public ICollection<CaseNoteEntity> Notes { get; set; } = [];
    public ICollection<CaseLinkEntity> Links { get; set; } = [];
}
