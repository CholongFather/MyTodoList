namespace DevTodoList.Shared.Enums;

/// <summary>링크 유형</summary>
public enum LinkType
{
    Jira = 0,
    Confluence = 1,
    GitHub = 2,
    GitLab = 3,
    Custom = 4
}

public static class LinkTypeExtensions
{
    public static string ToKorean(this LinkType l) => l switch
    {
        LinkType.Jira => "Jira",
        LinkType.Confluence => "Confluence",
        LinkType.GitHub => "GitHub",
        LinkType.GitLab => "GitLab",
        LinkType.Custom => "기타",
        _ => l.ToString()
    };
}
