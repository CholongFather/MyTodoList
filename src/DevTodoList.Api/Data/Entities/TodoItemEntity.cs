using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>TODO 항목 엔티티</summary>
[Table("TodoItems")]
public class TodoItemEntity : EntityBase
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    /// <summary>상태: Todo=0, InProgress=1, Done=2, Archived=3</summary>
    public int Status { get; set; }

    /// <summary>우선순위: Low=0, Normal=1, High=2, Urgent=3</summary>
    public int Priority { get; set; } = 1;

    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int SortOrder { get; set; }

    // 팀/담당자
    public long? TeamId { get; set; }
    public long? WorkCategoryId { get; set; }
    /// <summary>담당자 유형 동작: Mine=0, Others=1 (AssigneeTypeEntity.IsMine 기반 자동 설정)</summary>
    public int AssigneeType { get; set; }

    /// <summary>담당 유형 FK (설정에서 관리)</summary>
    public long? AssigneeTypeId { get; set; }

    /// <summary>외부 일정 여부</summary>
    public bool IsExternal { get; set; }

    /// <summary>외부 채널명 (외부 일정인 경우)</summary>
    public string? ExternalLabel { get; set; }

    // 계획 vs 실제 일정
    /// <summary>계획 시작일 (생성 시 고정)</summary>
    public DateTime? PlannedStartDate { get; set; }
    /// <summary>계획 종료일 (생성 시 고정)</summary>
    public DateTime? PlannedEndDate { get; set; }
    /// <summary>실제 시작일 (InProgress 전환 시 자동)</summary>
    public DateTime? ActualStartDate { get; set; }
    /// <summary>실제 종료일 (Done 전환 시 자동)</summary>
    public DateTime? ActualEndDate { get; set; }

    public long ProjectId { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public ProjectEntity Project { get; set; } = null!;

    [ForeignKey(nameof(TeamId))]
    public TeamEntity? Team { get; set; }

    [ForeignKey(nameof(WorkCategoryId))]
    public WorkCategoryEntity? WorkCategory { get; set; }

    [ForeignKey(nameof(AssigneeTypeId))]
    public AssigneeTypeEntity? AssigneeTypeEntity { get; set; }

    public ICollection<TodoTagEntity> TodoTags { get; set; } = [];
    public ICollection<TodoWorkerEntity> TodoWorkers { get; set; } = [];
    public ICollection<TodoCheckItemEntity> CheckItems { get; set; } = [];
    public ICollection<TodoLinkEntity> Links { get; set; } = [];
}
