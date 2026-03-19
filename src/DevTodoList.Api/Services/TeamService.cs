using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>팀/작업분류 관리 서비스</summary>
public class TeamService(AppDbContext db)
{
    public async Task<List<TeamDto>> GetAllAsync(CancellationToken ct = default)
    {
        var teams = await db.Teams
            .Include(t => t.WorkCategories)
            .OrderBy(t => t.SortOrder)
            .ToListAsync(ct);
        return teams.Select(t => t.ToDto()).ToList();
    }

    public async Task<TeamDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var team = await db.Teams
            .Include(t => t.WorkCategories)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
        return team?.ToDto();
    }

    public async Task<TeamDto> CreateAsync(CreateTeamRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.Teams.MaxAsync(t => (int?)t.SortOrder, ct) ?? 0;
        var entity = new TeamEntity
        {
            Name = req.Name,
            Color = req.Color,
            IsMine = req.IsMine,
            SortOrder = maxOrder + 1
        };
        db.Teams.Add(entity);
        await db.SaveChangesAsync(ct);
        return (await GetByIdAsync(entity.Id, ct))!;
    }

    public async Task<TeamDto?> UpdateAsync(long id, CreateTeamRequest req, CancellationToken ct = default)
    {
        var entity = await db.Teams.FindAsync([id], ct);
        if (entity is null) return null;
        entity.Name = req.Name;
        entity.Color = req.Color;
        entity.IsMine = req.IsMine;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Teams.FindAsync([id], ct);
        if (entity is null) return false;
        db.Teams.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    // === 작업 분류 ===

    public async Task<WorkCategoryDto> CreateCategoryAsync(long teamId, CreateWorkCategoryRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.WorkCategories
            .Where(c => c.TeamId == teamId)
            .MaxAsync(c => (int?)c.SortOrder, ct) ?? 0;
        var entity = new WorkCategoryEntity
        {
            Name = req.Name,
            TeamId = teamId,
            SortOrder = maxOrder + 1
        };
        db.WorkCategories.Add(entity);
        await db.SaveChangesAsync(ct);

        // Include Team for DTO mapping
        await db.Entry(entity).Reference(e => e.Team).LoadAsync(ct);
        return entity.ToDto();
    }

    public async Task<WorkCategoryDto?> UpdateCategoryAsync(long id, CreateWorkCategoryRequest req, CancellationToken ct = default)
    {
        var entity = await db.WorkCategories.Include(c => c.Team).FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return null;
        entity.Name = req.Name;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteCategoryAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.WorkCategories.FindAsync([id], ct);
        if (entity is null) return false;
        db.WorkCategories.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
