using DevTodoList.Shared.DTOs;

namespace DevTodoList.Shared.Helpers;

/// <summary>URL 패턴으로 링크 유형 자동 감지</summary>
public static class LinkTypeDetector
{
    /// <summary>도메인 키워드 → 링크 유형 이름 매핑</summary>
    private static readonly (string Pattern, string TypeName)[] Rules =
    [
        ("jira", "Jira"),
        ("atlassian.net/browse", "Jira"),
        ("atlassian.net/jira", "Jira"),
        ("confluence", "Confluence"),
        ("notion.so", "Notion"),
        ("notion.site", "Notion"),
        ("github.com", "GitHub"),
        ("gitlab.com", "GitLab"),
        ("gitlab.", "GitLab"),
        ("figma.com", "Figma"),
        ("slack.com", "Slack"),
    ];

    /// <summary>URL에서 매칭되는 LinkType을 찾아 ID 반환 (없으면 null)</summary>
    public static long? Detect(string? url, List<LinkTypeDto> linkTypes)
    {
        if (string.IsNullOrWhiteSpace(url) || linkTypes.Count == 0)
            return null;

        var lower = url.ToLowerInvariant();
        foreach (var (pattern, typeName) in Rules)
        {
            if (!lower.Contains(pattern)) continue;
            var match = linkTypes.FirstOrDefault(lt =>
                lt.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            if (match != null) return match.Id;
        }

        return null;
    }
}
