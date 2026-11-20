namespace DevTodoList.Shared.Enums;

/// <summary>프로젝트 상태</summary>
public enum ProjectStatus
{
    /// <summary>진행중</summary>
    Active = 0,
    /// <summary>라이브 (배포 완료)</summary>
    Live = 1,
    /// <summary>보관됨</summary>
    Archived = 2
}
