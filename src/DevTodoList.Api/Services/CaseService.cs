using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>케이스(이슈) 추적 CRUD 서비스</summary>
public class CaseService(AppDbContext db)
{
    // === 케이스 CRUD ===

    /// <summary>공통 필터 쿼리 빌더</summary>
    private IQueryable<CaseEntity> BuildFilteredQuery(
        CaseStatus? status, CaseCategory? category,
        long? environmentId, long? projectId, string? search)
    {
        var query = db.Cases
            .AsNoTracking()
            .Include(c => c.Project)
            .Include(c => c.Notes).ThenInclude(n => n.NoteTypeEntity)
            .Include(c => c.Links).ThenInclude(l => l.LinkTypeEntity)
            .Include(c => c.EnvironmentEntity)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.CaseStatus == (int)status.Value);
        if (category.HasValue)
            query = query.Where(c => c.CaseCategory == (int)category.Value);
        if (environmentId.HasValue)
            query = query.Where(c => c.EnvironmentId == environmentId.Value);
        if (projectId.HasValue)
            query = query.Where(c => c.ProjectId == projectId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Title.Contains(search) || (c.Description != null && c.Description.Contains(search)));

        return query;
    }

    public async Task<List<CaseDto>> GetAllAsync(
        CaseStatus? status = null, CaseCategory? category = null,
        long? environmentId = null, long? projectId = null,
        string? search = null, CancellationToken ct = default)
    {
        return await BuildFilteredQuery(status, category, environmentId, projectId, search)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => c.ToDto())
            .ToListAsync(ct);
    }

    /// <summary>페이지네이션 조회</summary>
    public async Task<PagedResult<CaseDto>> GetPagedAsync(
        CaseStatus? status = null, CaseCategory? category = null,
        long? environmentId = null, long? projectId = null,
        string? search = null, int page = PaginationDefaults.Page, int pageSize = PaginationDefaults.PageSize,
        CancellationToken ct = default)
    {
        var query = BuildFilteredQuery(status, category, environmentId, projectId, search);
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.ToDto())
            .ToListAsync(ct);

        return new PagedResult<CaseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<CaseDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Cases
            .Include(c => c.Project)
            .Include(c => c.Notes).ThenInclude(n => n.NoteTypeEntity)
            .Include(c => c.Links).ThenInclude(l => l.LinkTypeEntity)
            .Include(c => c.EnvironmentEntity)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        return entity?.ToDto();
    }

    public async Task<CaseDto> CreateAsync(CreateCaseRequest req, CancellationToken ct = default)
    {
        var entity = new CaseEntity
        {
            Title = req.Title,
            Description = req.Description,
            CaseStatus = (int)CaseStatus.Open,
            CaseCategory = (int)req.CaseCategory,
            EnvironmentId = req.EnvironmentId,
            Priority = req.Priority,
            Reporter = req.Reporter,
            Assignee = req.Assignee,
            JiraUrl = req.JiraUrl,
            WideUrl = req.WideUrl,
            ProjectId = req.ProjectId
        };
        db.Cases.Add(entity);
        await db.SaveChangesAsync(ct);

        // Navigation 로드
        if (entity.ProjectId.HasValue)
            await db.Entry(entity).Reference(c => c.Project).LoadAsync(ct);
        if (entity.EnvironmentId.HasValue)
            await db.Entry(entity).Reference(c => c.EnvironmentEntity).LoadAsync(ct);

        return entity.ToDto();
    }

    public async Task<CaseDto?> UpdateAsync(long id, CreateCaseRequest req, CancellationToken ct = default)
    {
        var entity = await db.Cases
            .Include(c => c.Project)
            .Include(c => c.Notes)
            .Include(c => c.Links)
            .Include(c => c.EnvironmentEntity)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return null;

        entity.Title = req.Title;
        entity.Description = req.Description;
        entity.CaseCategory = (int)req.CaseCategory;
        entity.EnvironmentId = req.EnvironmentId;
        entity.Priority = req.Priority;
        entity.Reporter = req.Reporter;
        entity.Assignee = req.Assignee;
        entity.JiraUrl = req.JiraUrl;
        entity.WideUrl = req.WideUrl;
        entity.ProjectId = req.ProjectId;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Cases.Include(c => c.Notes).Include(c => c.Links).FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return false;
        db.CaseLinks.RemoveRange(entity.Links);
        db.CaseNotes.RemoveRange(entity.Notes);
        db.Cases.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<CaseDto?> UpdateStatusAsync(long id, UpdateCaseStatusRequest req, CancellationToken ct = default)
    {
        var entity = await db.Cases
            .Include(c => c.Project)
            .Include(c => c.Notes)
            .Include(c => c.Links)
            .Include(c => c.EnvironmentEntity)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return null;

        entity.CaseStatus = (int)req.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        // Resolved 상태 전환 시 ResolvedAt 기록
        if (req.Status == CaseStatus.Resolved && entity.ResolvedAt is null)
            entity.ResolvedAt = DateTime.UtcNow;
        else if (req.Status < CaseStatus.Resolved)
            entity.ResolvedAt = null;

        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    /// <summary>상태별/카테고리별 통계 (DB 레벨 그룹핑)</summary>
    public async Task<object> GetStatsAsync(CancellationToken ct = default)
    {
        var total = await db.Cases.AsNoTracking().CountAsync(ct);
        var byStatus = await db.Cases.AsNoTracking()
            .GroupBy(c => c.CaseStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);
        var byCategory = await db.Cases.AsNoTracking()
            .GroupBy(c => c.CaseCategory)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return new
        {
            Total = total,
            ByStatus = byStatus.ToDictionary(x => ((CaseStatus)x.Status).ToString(), x => x.Count),
            ByCategory = byCategory.ToDictionary(x => ((CaseCategory)x.Category).ToString(), x => x.Count)
        };
    }

    // === 노트 CRUD ===

    public async Task<CaseNoteDto> CreateNoteAsync(long caseId, CreateCaseNoteRequest req, CancellationToken ct = default)
    {
        var entity = new CaseNoteEntity
        {
            CaseId = caseId,
            Content = req.Content,
            NoteType = (int)req.NoteType,
            NoteTypeId = req.NoteTypeId,
            Author = req.Author
        };
        db.CaseNotes.Add(entity);
        await db.SaveChangesAsync(ct);
        // Navigation 로드
        if (entity.NoteTypeId.HasValue)
            await db.Entry(entity).Reference(n => n.NoteTypeEntity).LoadAsync(ct);
        return entity.ToDto();
    }

    public async Task<CaseNoteDto?> UpdateNoteAsync(long noteId, CreateCaseNoteRequest req, CancellationToken ct = default)
    {
        var entity = await db.CaseNotes.FindAsync([noteId], ct);
        if (entity is null) return null;

        entity.Content = req.Content;
        entity.NoteType = (int)req.NoteType;
        entity.NoteTypeId = req.NoteTypeId;
        entity.Author = req.Author;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        if (entity.NoteTypeId.HasValue)
            await db.Entry(entity).Reference(n => n.NoteTypeEntity).LoadAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteNoteAsync(long noteId, CancellationToken ct = default)
    {
        var entity = await db.CaseNotes.FindAsync([noteId], ct);
        if (entity is null) return false;
        db.CaseNotes.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    // === 링크 CRUD ===

    public async Task<CaseLinkDto> CreateLinkAsync(long caseId, CreateLinkRequest req, CancellationToken ct = default)
    {
        var entity = new CaseLinkEntity
        {
            CaseId = caseId,
            Title = req.Title,
            Url = req.Url,
            LinkType = (int)req.LinkType,
            LinkTypeId = req.LinkTypeId
        };
        db.CaseLinks.Add(entity);
        await db.SaveChangesAsync(ct);
        if (entity.LinkTypeId.HasValue)
            await db.Entry(entity).Reference(l => l.LinkTypeEntity).LoadAsync(ct);
        return entity.ToDto();
    }

    public async Task<CaseLinkDto?> UpdateLinkAsync(long linkId, CreateLinkRequest req, CancellationToken ct = default)
    {
        var entity = await db.CaseLinks.FindAsync([linkId], ct);
        if (entity is null) return null;
        entity.Title = req.Title;
        entity.Url = req.Url;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteLinkAsync(long linkId, CancellationToken ct = default)
    {
        var entity = await db.CaseLinks.FindAsync([linkId], ct);
        if (entity is null) return false;
        db.CaseLinks.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
