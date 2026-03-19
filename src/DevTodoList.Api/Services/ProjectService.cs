using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>프로젝트 서비스</summary>
public class ProjectService(AppDbContext db)
{
    public async Task<List<ProjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await db.Projects
            .Include(p => p.TodoItems)
            .Include(p => p.Metas)
            .OrderBy(p => p.SortOrder)
            .Select(p => p.ToDto())
            .ToListAsync(ct);
    }

    public async Task<ProjectDto?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Projects
            .Include(p => p.TodoItems)
            .Include(p => p.Metas)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return entity?.ToDto();
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.Projects.MaxAsync(p => (int?)p.SortOrder, ct) ?? 0;
        var entity = new ProjectEntity
        {
            Name = req.Name,
            Description = req.Description,
            Color = req.Color,
            SortOrder = maxOrder + 1
        };
        db.Projects.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<ProjectDto?> UpdateAsync(long id, CreateProjectRequest req, CancellationToken ct = default)
    {
        var entity = await db.Projects.Include(p => p.TodoItems).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity is null) return null;

        entity.Name = req.Name;
        entity.Description = req.Description;
        entity.Color = req.Color;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Projects.FindAsync([id], ct);
        if (entity is null) return false;
        db.Projects.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task ReorderAsync(ReorderRequest req, CancellationToken ct = default)
    {
        for (int i = 0; i < req.OrderedIds.Count; i++)
        {
            var entity = await db.Projects.FindAsync([req.OrderedIds[i]], ct);
            if (entity is not null)
            {
                entity.SortOrder = i;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        await db.SaveChangesAsync(ct);
    }

    // === 프로젝트 메타 CRUD ===

    public async Task<ProjectMetaDto> CreateMetaAsync(long projectId, CreateProjectMetaRequest req, CancellationToken ct = default)
    {
        var maxOrder = await db.ProjectMetas.Where(m => m.ProjectId == projectId).MaxAsync(m => (int?)m.SortOrder, ct) ?? 0;
        var entity = new ProjectMetaEntity
        {
            ProjectId = projectId,
            MetaType = (int)req.MetaType,
            Label = req.Label,
            Value = req.Value,
            SortOrder = maxOrder + 1
        };
        db.ProjectMetas.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<ProjectMetaDto?> UpdateMetaAsync(long metaId, CreateProjectMetaRequest req, CancellationToken ct = default)
    {
        var entity = await db.ProjectMetas.FindAsync([metaId], ct);
        if (entity is null) return null;
        entity.MetaType = (int)req.MetaType;
        entity.Label = req.Label;
        entity.Value = req.Value;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteMetaAsync(long metaId, CancellationToken ct = default)
    {
        var entity = await db.ProjectMetas.FindAsync([metaId], ct);
        if (entity is null) return false;
        db.ProjectMetas.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
