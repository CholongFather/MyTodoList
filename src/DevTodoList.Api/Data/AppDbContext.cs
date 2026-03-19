using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProjectEntity> Projects => Set<ProjectEntity>();
    public DbSet<TodoItemEntity> TodoItems => Set<TodoItemEntity>();
    public DbSet<TagEntity> Tags => Set<TagEntity>();
    public DbSet<TodoTagEntity> TodoTags => Set<TodoTagEntity>();
    public DbSet<TodoCheckItemEntity> TodoCheckItems => Set<TodoCheckItemEntity>();
    public DbSet<TodoLinkEntity> TodoLinks => Set<TodoLinkEntity>();
    public DbSet<NotificationSettingEntity> NotificationSettings => Set<NotificationSettingEntity>();
    public DbSet<BusCommuteSettingEntity> BusCommuteSettings => Set<BusCommuteSettingEntity>();
    public DbSet<ProjectMetaEntity> ProjectMetas => Set<ProjectMetaEntity>();
    public DbSet<CaseEntity> Cases => Set<CaseEntity>();
    public DbSet<CaseNoteEntity> CaseNotes => Set<CaseNoteEntity>();
    public DbSet<CaseLinkEntity> CaseLinks => Set<CaseLinkEntity>();
    public DbSet<TeamEntity> Teams => Set<TeamEntity>();
    public DbSet<WorkCategoryEntity> WorkCategories => Set<WorkCategoryEntity>();
    public DbSet<EnvironmentEntity> Environments => Set<EnvironmentEntity>();
    public DbSet<HolidayEntity> Holidays => Set<HolidayEntity>();
    public DbSet<AssigneeTypeEntity> AssigneeTypes => Set<AssigneeTypeEntity>();
    public DbSet<BusStationEntity> BusStations => Set<BusStationEntity>();
    public DbSet<WorkerEntity> Workers => Set<WorkerEntity>();
    public DbSet<TodoWorkerEntity> TodoWorkers => Set<TodoWorkerEntity>();
    public DbSet<NoteTypeEntity> NoteTypes => Set<NoteTypeEntity>();
    public DbSet<LinkTypeEntity> LinkTypes => Set<LinkTypeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TodoTag 복합 키
        modelBuilder.Entity<TodoTagEntity>(e =>
        {
            e.HasKey(x => new { x.TodoItemId, x.TagId });
        });

        // TodoWorker 복합 키
        modelBuilder.Entity<TodoWorkerEntity>(e =>
        {
            e.HasKey(x => new { x.TodoItemId, x.WorkerId });
        });

        // Worker 유니크 인덱스
        modelBuilder.Entity<WorkerEntity>(e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
        });

        // 유니크 인덱스
        modelBuilder.Entity<ProjectEntity>(e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<TagEntity>(e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<TeamEntity>(e =>
        {
            e.HasIndex(x => x.Name).IsUnique();
        });

        // 조회 성능 인덱스
        modelBuilder.Entity<TodoItemEntity>(e =>
        {
            e.HasIndex(x => new { x.ProjectId, x.Status });
            e.HasIndex(x => x.DueDate);
            e.HasIndex(x => new { x.StartDate, x.EndDate });
            e.HasIndex(x => x.TeamId);
            e.HasIndex(x => x.AssigneeType);
        });

        modelBuilder.Entity<TodoCheckItemEntity>(e =>
        {
            e.HasIndex(x => new { x.TodoItemId, x.SortOrder });
        });

        modelBuilder.Entity<BusCommuteSettingEntity>(e =>
        {
            e.HasIndex(x => x.IsEnabled);
        });

        modelBuilder.Entity<ProjectMetaEntity>(e =>
        {
            e.HasIndex(x => new { x.ProjectId, x.MetaType });
        });

        // Case 인덱스
        modelBuilder.Entity<CaseEntity>(e =>
        {
            e.HasIndex(x => x.CaseStatus);
            e.HasIndex(x => x.ProjectId);
            e.HasIndex(x => new { x.CaseCategory, x.Environment });
        });

        modelBuilder.Entity<CaseNoteEntity>(e =>
        {
            e.HasIndex(x => x.CaseId);
        });

        modelBuilder.Entity<CaseLinkEntity>(e =>
        {
            e.HasIndex(x => x.CaseId);
        });

        // Holiday 인덱스
        modelBuilder.Entity<HolidayEntity>(e =>
        {
            e.HasIndex(x => x.Date);
        });

        // 기본 알림 설정 시드
        modelBuilder.Entity<NotificationSettingEntity>().HasData(new NotificationSettingEntity
        {
            Id = 1,
            IsEnabled = true,
            NotificationTime = NotificationDefaults.NotificationTime,
            DashboardRefreshIntervalSeconds = NotificationDefaults.DashboardRefreshIntervalSeconds,
            DueSoonDays = NotificationDefaults.DueSoonDays,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
