using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>프로젝트 메타 정보 (URL, 버전, 비밀번호 등)</summary>
[Table("ProjectMetas")]
public class ProjectMetaEntity : EntityBase
{
    public long ProjectId { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public ProjectEntity Project { get; set; } = null!;

    /// <summary>메타 유형: 0=Url, 1=Version, 2=Credential</summary>
    public int MetaType { get; set; }

    /// <summary>라벨 (기획서, Axure, Git 저장소, 테스트 계정 등)</summary>
    [Required, MaxLength(100)]
    public string Label { get; set; } = string.Empty;

    /// <summary>값 (URL, 버전 문자열, 비밀번호 등)</summary>
    [Required, MaxLength(1000)]
    public string Value { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}
