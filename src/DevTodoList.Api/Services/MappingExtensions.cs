using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.Constants;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.Enums;

namespace DevTodoList.Api.Services;

/// <summary>Entity ↔ DTO 매핑 확장 메서드</summary>
public static class MappingExtensions
{
    public static ProjectDto ToDto(this ProjectEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        Color = e.Color,
        SortOrder = e.SortOrder,
        IsArchived = e.IsArchived,
        TodoCount = e.TodoItems?.Count ?? 0,
        ActiveCount = e.TodoItems?.Count(t => t.Status < (int)TodoStatus.Done) ?? 0,
        CreatedAt = e.CreatedAt,
        Metas = e.Metas?.OrderBy(m => m.SortOrder).Select(m => m.ToDto()).ToList() ?? []
    };

    public static ProjectMetaDto ToDto(this ProjectMetaEntity e) => new()
    {
        Id = e.Id,
        ProjectId = e.ProjectId,
        MetaType = (ProjectMetaType)e.MetaType,
        Label = e.Label,
        Value = e.Value,
        SortOrder = e.SortOrder
    };

    public static TodoItemDto ToDto(this TodoItemEntity e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        Status = (TodoStatus)e.Status,
        Priority = (TodoPriority)e.Priority,
        DueDate = e.DueDate,
        StartDate = e.StartDate,
        EndDate = e.EndDate,
        CompletedAt = e.CompletedAt,
        TeamId = e.TeamId,
        TeamName = e.Team?.Name,
        TeamColor = e.Team?.Color,
        WorkCategoryId = e.WorkCategoryId,
        WorkCategoryName = e.WorkCategory?.Name,
        AssigneeType = e.AssigneeTypeEntity != null
            ? (e.AssigneeTypeEntity.IsMine ? AssigneeType.Mine : AssigneeType.Others)
            : (AssigneeType)e.AssigneeType,
        AssigneeTypeId = e.AssigneeTypeId,
        AssigneeTypeName = e.AssigneeTypeEntity?.Name,
        AssigneeTypeColor = e.AssigneeTypeEntity?.Color,
        IsExternal = e.IsExternal,
        ExternalLabel = e.ExternalLabel,
        PlannedStartDate = e.PlannedStartDate,
        PlannedEndDate = e.PlannedEndDate,
        ActualStartDate = e.ActualStartDate,
        ActualEndDate = e.ActualEndDate,
        SortOrder = e.SortOrder,
        ProjectId = e.ProjectId,
        ProjectName = e.Project?.Name ?? string.Empty,
        ProjectColor = e.Project?.Color ?? AppColors.DefaultProject,
        Tags = e.TodoTags?.Select(t => t.Tag.ToDto()).ToList() ?? [],
        Workers = e.TodoWorkers?.Select(w => w.Worker.ToDto()).ToList() ?? [],
        CheckItems = e.CheckItems?.OrderBy(c => c.SortOrder).Select(c => c.ToDto()).ToList() ?? [],
        Links = e.Links?.Select(l => l.ToDto()).ToList() ?? [],
        CheckItemTotal = e.CheckItems?.Count ?? 0,
        CheckItemCompleted = e.CheckItems?.Count(c => c.IsCompleted) ?? 0,
        CreatedAt = e.CreatedAt
    };

    public static List<TodoItemDto> ToDtoList(this IEnumerable<TodoItemEntity> entities)
        => entities.Select(e => e.ToDto()).ToList();

    public static TagDto ToDto(this TagEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Color = e.Color
    };

    public static TodoCheckItemDto ToDto(this TodoCheckItemEntity e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        IsCompleted = e.IsCompleted,
        SortOrder = e.SortOrder,
        TodoItemId = e.TodoItemId
    };

    public static TodoLinkDto ToDto(this TodoLinkEntity e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Url = e.Url,
        LinkType = (LinkType)e.LinkType,
        LinkTypeId = e.LinkTypeId,
        LinkTypeName = e.LinkTypeEntity?.Name,
        LinkTypeColor = e.LinkTypeEntity?.Color,
        TodoItemId = e.TodoItemId
    };

    public static NotificationSettingDto ToDto(this NotificationSettingEntity e) => new()
    {
        IsEnabled = e.IsEnabled,
        NotificationTime = e.NotificationTime,
        DashboardRefreshIntervalSeconds = e.DashboardRefreshIntervalSeconds,
        DueSoonDays = e.DueSoonDays,
        GbisApiKey = e.GbisApiKey,
        ScheduleWebhookUrl = e.ScheduleWebhookUrl,
        BusWebhookUrl = e.BusWebhookUrl
    };

    public static BusCommuteSettingDto ToDto(this BusCommuteSettingEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        CommuteType = (CommuteType)e.CommuteType,
        BusStopName = e.BusStation?.BusStopName ?? e.BusStopName,
        BusRouteNumber = e.BusStation?.BusRouteNumber ?? e.BusRouteNumber,
        GbisStationId = e.BusStation?.GbisStationId ?? e.GbisStationId,
        GbisRouteId = e.BusStation?.GbisRouteId ?? e.GbisRouteId,
        GbisStaOrder = e.BusStation?.GbisStaOrder ?? e.GbisStaOrder,
        MonitoringStartTime = e.MonitoringStartTime,
        MonitoringEndTime = e.MonitoringEndTime,
        AlertMinutesBefore = e.AlertMinutesBefore,
        AlertThresholds = e.AlertThresholds,
        IsEnabled = e.IsEnabled,
        ActiveDays = e.ActiveDays,
        LastNotifiedAt = e.LastNotifiedAt,
        CreatedAt = e.CreatedAt,
        BusStationId = e.BusStationId,
        BusStationName = e.BusStation?.Name
    };

    public static BusStationDto ToDto(this BusStationEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        BusStopName = e.BusStopName,
        BusRouteNumber = e.BusRouteNumber,
        GbisStationId = e.GbisStationId,
        GbisRouteId = e.GbisRouteId,
        GbisStaOrder = e.GbisStaOrder,
        Direction = (BusDirection)e.Direction,
        SortOrder = e.SortOrder
    };

    public static CaseDto ToDto(this CaseEntity e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        CaseStatus = (CaseStatus)e.CaseStatus,
        CaseCategory = (CaseCategory)e.CaseCategory,
        EnvironmentId = e.EnvironmentId,
        EnvironmentName = e.EnvironmentEntity?.Name,
        EnvironmentColor = e.EnvironmentEntity?.Color,
        Priority = e.Priority,
        Reporter = e.Reporter,
        Assignee = e.Assignee,
        JiraUrl = e.JiraUrl,
        WideUrl = e.WideUrl,
        ProjectId = e.ProjectId,
        ProjectName = e.Project?.Name,
        ProjectColor = e.Project?.Color,
        ResolvedAt = e.ResolvedAt,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
        Notes = e.Notes?.OrderByDescending(n => n.CreatedAt).Select(n => n.ToDto()).ToList() ?? [],
        NoteCount = e.Notes?.Count ?? 0,
        Links = e.Links?.Select(l => l.ToDto()).ToList() ?? []
    };

    public static CaseLinkDto ToDto(this CaseLinkEntity e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Url = e.Url,
        LinkType = (LinkType)e.LinkType,
        LinkTypeId = e.LinkTypeId,
        LinkTypeName = e.LinkTypeEntity?.Name,
        LinkTypeColor = e.LinkTypeEntity?.Color,
        CaseId = e.CaseId
    };

    public static CaseNoteDto ToDto(this CaseNoteEntity e) => new()
    {
        Id = e.Id,
        CaseId = e.CaseId,
        Content = e.Content,
        NoteType = (CaseNoteType)e.NoteType,
        NoteTypeId = e.NoteTypeId,
        NoteTypeName = e.NoteTypeEntity?.Name,
        NoteTypeColor = e.NoteTypeEntity?.Color,
        NoteTypeIcon = e.NoteTypeEntity?.Icon,
        Author = e.Author,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };

    public static TeamDto ToDto(this TeamEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Color = e.Color,
        SortOrder = e.SortOrder,
        IsMine = e.IsMine,
        WorkCategories = e.WorkCategories?.OrderBy(c => c.SortOrder).Select(c => c.ToDto()).ToList() ?? []
    };

    public static WorkCategoryDto ToDto(this WorkCategoryEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        SortOrder = e.SortOrder,
        TeamId = e.TeamId,
        TeamName = e.Team?.Name ?? string.Empty
    };

    public static HolidayDto ToDto(this HolidayEntity e) => new()
    {
        Id = e.Id,
        Date = e.Date,
        Name = e.Name,
        IsRecurring = e.IsRecurring,
        HolidayType = (HolidayType)e.HolidayType
    };

    public static WorkerDto ToDto(this WorkerEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Color = e.Color,
        IsMe = e.IsMe,
        SortOrder = e.SortOrder
    };

    public static NoteTypeDto ToDto(this NoteTypeEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Color = e.Color,
        Icon = e.Icon,
        SortOrder = e.SortOrder
    };

    public static LinkTypeDto ToDto(this LinkTypeEntity e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Color = e.Color,
        Icon = e.Icon,
        SortOrder = e.SortOrder
    };

    public static GanttItemDto ToGanttDto(this TodoItemEntity e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        StartDate = e.StartDate ?? e.CreatedAt,
        EndDate = e.EndDate ?? e.DueDate ?? e.CreatedAt.AddDays(1),
        Status = (TodoStatus)e.Status,
        Priority = (TodoPriority)e.Priority,
        ProjectId = e.ProjectId,
        ProjectName = e.Project?.Name ?? string.Empty,
        ProjectColor = e.Project?.Color ?? AppColors.DefaultProject,
        ProgressPercent = e.CheckItems?.Count > 0
            ? Math.Round(e.CheckItems.Count(c => c.IsCompleted) * 100.0 / e.CheckItems.Count, 1)
            : 0,
        PlannedStartDate = e.PlannedStartDate,
        PlannedEndDate = e.PlannedEndDate,
        ActualStartDate = e.ActualStartDate,
        ActualEndDate = e.ActualEndDate,
        TeamId = e.TeamId,
        TeamName = e.Team?.Name,
        WorkCategoryId = e.WorkCategoryId,
        WorkCategoryName = e.WorkCategory?.Name,
        AssigneeType = e.AssigneeTypeEntity != null
            ? (e.AssigneeTypeEntity.IsMine ? AssigneeType.Mine : AssigneeType.Others)
            : (AssigneeType)e.AssigneeType,
        AssigneeTypeId = e.AssigneeTypeId,
        AssigneeTypeName = e.AssigneeTypeEntity?.Name,
        AssigneeTypeColor = e.AssigneeTypeEntity?.Color,
        IsExternal = e.IsExternal,
        ExternalLabel = e.ExternalLabel,
        Workers = e.TodoWorkers?.Select(w => w.Worker.ToDto()).ToList() ?? [],
        IsDelayed = e.ActualEndDate.HasValue && e.PlannedEndDate.HasValue && e.ActualEndDate > e.PlannedEndDate,
        DelayDays = e.ActualEndDate.HasValue && e.PlannedEndDate.HasValue && e.ActualEndDate > e.PlannedEndDate
            ? (e.ActualEndDate.Value - e.PlannedEndDate.Value).Days : 0
    };
}
