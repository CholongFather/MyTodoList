namespace DevTodoList.Shared.Enums;

/// <summary>프로젝트 메타 유형</summary>
public enum ProjectMetaType
{
    /// <summary>URL (기획서, Axure, Git 등)</summary>
    Url = 0,
    /// <summary>버전 정보</summary>
    Version = 1,
    /// <summary>비밀번호/인증 정보</summary>
    Credential = 2,
    /// <summary>외부 채널 (프로젝트에 관련된 외부 팀/채널 정보)</summary>
    ExternalChannel = 3
}

public static class ProjectMetaTypeExtensions
{
    public static string ToKorean(this ProjectMetaType t) => t switch
    {
        ProjectMetaType.Url => "URL",
        ProjectMetaType.Version => "버전",
        ProjectMetaType.Credential => "인증정보",
        ProjectMetaType.ExternalChannel => "외부채널",
        _ => t.ToString()
    };
}
