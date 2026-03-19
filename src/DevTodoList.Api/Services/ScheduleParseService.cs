using System.Text.RegularExpressions;
using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>일정 텍스트 파싱 서비스</summary>
public class ScheduleParseService(AppDbContext db)
{
    // 날짜 패턴: "2026. 3. 5." 또는 "2026.3.5" 또는 "3월 3주 초" 등
    private static readonly Regex DatePattern = new(
        @"(\d{4})\s*\.\s*(\d{1,2})\s*\.\s*(\d{1,2})\s*\.?",
        RegexOptions.Compiled);

    /// <summary>텍스트를 파싱하여 일정 항목 리스트 반환 (미리보기용)</summary>
    public List<ScheduleParsedItemDto> Parse(string text)
    {
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<ScheduleParsedItemDto>();
        string? currentParent = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 들여쓰기 깊이 계산
            var trimmed = line.TrimStart();
            var indent = line.Length - trimmed.Length;
            var depth = indent >= 2 ? 1 : 0;

            // 콜론으로 제목과 날짜 분리
            var colonIdx = trimmed.IndexOf(':');
            if (colonIdx <= 0)
            {
                // 콜론 없으면 카테고리 제목으로 간주
                currentParent = trimmed.Trim();
                // "개발", "QA" 같은 섹션 헤더도 포함
                if (!string.IsNullOrWhiteSpace(currentParent) && depth == 0)
                    continue; // 섹션 헤더는 건너뜀
                continue;
            }

            var title = trimmed[..colonIdx].Trim();
            var dateStr = trimmed[(colonIdx + 1)..].Trim();

            // 부모 제목이 없으면 depth 0
            if (depth == 0) currentParent = null;

            var item = new ScheduleParsedItemDto
            {
                Title = title,
                Depth = depth,
                ParentTitle = depth > 0 ? currentParent : null
            };

            // 날짜 패턴 파싱
            var dates = DatePattern.Matches(dateStr);

            if (dateStr.Contains('~'))
            {
                // 범위 패턴: "시작일 ~ 종료일" 또는 "~ 종료일"
                var parts = dateStr.Split('~');
                var startPart = parts[0].Trim();
                var endPart = parts.Length > 1 ? parts[1].Trim() : "";

                if (!string.IsNullOrEmpty(startPart))
                    item.StartDate = ParseDate(startPart);

                if (!string.IsNullOrEmpty(endPart))
                    item.EndDate = ParseDate(endPart);
            }
            else if (dates.Count == 1)
            {
                // 단일 날짜 = 마일스톤
                item.StartDate = ParseDate(dateStr);
                item.EndDate = item.StartDate;
                item.IsMilestone = true;
            }
            else if (dateStr.Contains("부터"))
            {
                // "~부터" 패턴
                item.StartDate = ParseDate(dateStr.Replace("부터", "").Trim());
                item.IsMilestone = true;
            }

            // 깊이 0이면서 콜론이 있으면 현재 부모로 설정
            if (depth == 0 && item.StartDate is null && item.EndDate is null)
            {
                currentParent = title;
                continue;
            }

            result.Add(item);
        }

        return result;
    }

    /// <summary>파싱된 항목을 TODO로 일괄 생성</summary>
    public async Task<List<TodoItemDto>> CreateFromParsedAsync(
        List<ScheduleParsedItemDto> items, long projectId,
        long? defaultTeamId, long? defaultWorkCategoryId, int defaultAssigneeType,
        long? defaultAssigneeTypeId = null, bool defaultIsExternal = false,
        CancellationToken ct = default)
    {
        var created = new List<TodoItemEntity>();
        var maxOrder = await db.TodoItems
            .Where(x => x.ProjectId == projectId)
            .MaxAsync(x => (int?)x.SortOrder, ct) ?? 0;

        foreach (var item in items)
        {
            maxOrder++;
            var entity = new TodoItemEntity
            {
                Title = item.ParentTitle is not null ? $"[{item.ParentTitle}] {item.Title}" : item.Title,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                DueDate = item.EndDate ?? item.StartDate,
                ProjectId = projectId,
                SortOrder = maxOrder,
                Priority = 1, // Normal
                TeamId = item.TeamId ?? defaultTeamId,
                WorkCategoryId = item.WorkCategoryId ?? defaultWorkCategoryId,
                AssigneeType = defaultAssigneeType,
                AssigneeTypeId = defaultAssigneeTypeId,
                PlannedStartDate = item.StartDate,
                PlannedEndDate = item.EndDate,
                IsExternal = defaultIsExternal,
            };
            db.TodoItems.Add(entity);
            created.Add(entity);
        }

        await db.SaveChangesAsync(ct);

        return created.Select(e =>
        {
            e.Project = db.Projects.Find(projectId)!;
            return e.ToDto();
        }).ToList();
    }

    /// <summary>프로젝트의 기존 TODO를 삭제하고 새 일정으로 교체</summary>
    public async Task<BulkReplaceResultDto> ReplaceFromTextAsync(
        string text, long projectId,
        long? defaultTeamId, long? defaultWorkCategoryId, int defaultAssigneeType,
        long? defaultAssigneeTypeId = null, bool defaultIsExternal = false,
        CancellationToken ct = default)
    {
        // 기존 TODO 삭제 (관련 자식 엔티티는 EF cascade로 자동 삭제)
        var existing = await db.TodoItems
            .Where(x => x.ProjectId == projectId)
            .ToListAsync(ct);
        var deletedCount = existing.Count;
        db.TodoItems.RemoveRange(existing);
        await db.SaveChangesAsync(ct);

        // 새 일정 파싱 및 생성
        var parsed = Parse(text);
        var created = parsed.Count > 0
            ? await CreateFromParsedAsync(parsed, projectId, defaultTeamId, defaultWorkCategoryId, defaultAssigneeType, defaultAssigneeTypeId, defaultIsExternal, ct)
            : [];

        return new BulkReplaceResultDto
        {
            DeletedCount = deletedCount,
            CreatedItems = created
        };
    }

    private static DateTime? ParseDate(string text)
    {
        var match = DatePattern.Match(text);
        if (!match.Success) return null;

        if (int.TryParse(match.Groups[1].Value, out var year) &&
            int.TryParse(match.Groups[2].Value, out var month) &&
            int.TryParse(match.Groups[3].Value, out var day))
        {
            try { return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc); }
            catch { return null; }
        }

        return null;
    }
}
