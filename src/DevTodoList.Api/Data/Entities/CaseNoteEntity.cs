using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>케이스 노트 (코멘트/가이드/워크어라운드/근본원인)</summary>
[Table("CaseNotes")]
public class CaseNoteEntity : EntityBase
{
    /// <summary>케이스 ID</summary>
    public long CaseId { get; set; }

    /// <summary>마크다운 텍스트 내용</summary>
    [Required]
    public string Content { get; set; } = string.Empty;

    /// <summary>노트 유형: Comment(0), Guide(1), Workaround(2), RootCause(3) - 레거시</summary>
    public int NoteType { get; set; }

    /// <summary>노트 유형 FK (DB 관리)</summary>
    public long? NoteTypeId { get; set; }

    /// <summary>작성자</summary>
    [MaxLength(100)]
    public string? Author { get; set; }

    // Navigation
    [ForeignKey(nameof(CaseId))]
    public CaseEntity Case { get; set; } = null!;

    [ForeignKey(nameof(NoteTypeId))]
    public NoteTypeEntity? NoteTypeEntity { get; set; }
}
