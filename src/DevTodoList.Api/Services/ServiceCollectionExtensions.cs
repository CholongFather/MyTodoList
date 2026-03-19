namespace DevTodoList.Api.Services;

/// <summary>서비스 DI 등록</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDevTodoServices(this IServiceCollection services)
    {
        services.AddScoped<ProjectService>();
        services.AddScoped<TodoService>();
        services.AddScoped<TagService>();
        services.AddScoped<CheckItemService>();
        services.AddScoped<LinkService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<GanttService>();
        services.AddScoped<NotificationSettingService>();
        services.AddScoped<ScheduleParseService>();
        services.AddScoped<BusCommuteService>();
        services.AddScoped<TeamsWebhookService>();
        services.AddScoped<GbisBusArrivalService>();
        services.AddScoped<CaseService>();
        services.AddScoped<TeamService>();
        services.AddScoped<EnvironmentService>();
        services.AddScoped<HolidayService>();
        services.AddScoped<AssigneeTypeService>();
        services.AddScoped<BusStationService>();
        services.AddScoped<WorkerService>();
        services.AddScoped<NoteTypeService>();
        services.AddScoped<LinkTypeService>();
        return services;
    }
}
