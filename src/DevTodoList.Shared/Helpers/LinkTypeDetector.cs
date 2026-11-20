using DevTodoList.Shared.DTOs;

namespace DevTodoList.Shared.Helpers;

/// <summary>URL 패턴으로 링크 유형 자동 감지 (DB 설정 기반)</summary>
public static class LinkTypeDetector
{
    /// <summary>URL에서 매칭되는 LinkType을 찾아 ID 반환 (없으면 null)</summary>
    public static long? Detect(string? url, List<LinkTypeDto> linkTypes)
    {
        if (string.IsNullOrWhiteSpace(url) || linkTypes.Count == 0)
            return null;

        var lower = url.ToLowerInvariant();
        foreach (var lt in linkTypes)
        {
            if (string.IsNullOrWhiteSpace(lt.UrlPattern)) continue;
            var patterns = lt.UrlPattern.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var pattern in patterns)
            {
                if (lower.Contains(pattern.ToLowerInvariant()))
                    return lt.Id;
            }
        }

        return null;
    }
}
