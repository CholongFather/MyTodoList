using DevTodoList.Shared.Enums;

namespace DevTodoList.Shared.DTOs.Requests;

/// <summary>케이스 노트 생성/수정 요청</summary>
public class CreateCaseNoteRequest
{
    public string Content { get; set; } = string.Empty;
    public CaseNoteType NoteType { get; set; }
    public long? NoteTypeId { get; set; }
    public string? Author { get; set; }
}
