using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs;

/// <summary>케이스 노트 응답 DTO</summary>
public class CaseNoteDto
{
    public long Id { get; set; }
    public long CaseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public CaseNoteType NoteType { get; set; }
    public long? NoteTypeId { get; set; }
    public string? NoteTypeName { get; set; }
    public string? NoteTypeColor { get; set; }
    public string? NoteTypeIcon { get; set; }
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
