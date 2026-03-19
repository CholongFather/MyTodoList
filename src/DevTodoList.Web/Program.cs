using DevTodoList.Web.Components;
using DevTodoList.Web.Services.ApiClient;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// IIS에서 bin/Release 직접 참조 시 정적 웹 에셋(MudBlazor CSS/JS 등) 로드 필요
// IIS 앱 풀 계정이 NuGet 캐시에 접근 못 할 수 있으므로 안전하게 시도
try
{
    builder.WebHost.UseStaticWebAssets();
}
catch (DirectoryNotFoundException)
{
    // IIS 환경에서 NuGet 캐시 접근 불가 시 무시 (publish된 wwwroot 사용)
}

// MudBlazor
builder.Services.AddMudServices();

// Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 상세 오류 표시 (appsettings.json의 DetailedErrors 설정)
if (builder.Configuration.GetValue<bool>("DetailedErrors"))
{
    builder.Services.Configure<Microsoft.AspNetCore.Components.Server.CircuitOptions>(o => o.DetailedErrors = true);
}

// API HttpClient
builder.Services.AddHttpClient("DevTodoApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5100");
});

// API 클라이언트 서비스
builder.Services.AddScoped<ProjectApiClient>();
builder.Services.AddScoped<TodoApiClient>();
builder.Services.AddScoped<TagApiClient>();
builder.Services.AddScoped<DashboardApiClient>();
builder.Services.AddScoped<GanttApiClient>();
builder.Services.AddScoped<ScheduleApiClient>();
builder.Services.AddScoped<SettingsApiClient>();
builder.Services.AddScoped<BusCommuteApiClient>();
builder.Services.AddScoped<CaseApiClient>();
builder.Services.AddScoped<TeamApiClient>();
builder.Services.AddScoped<EnvironmentApiClient>();
builder.Services.AddScoped<HolidayApiClient>();
builder.Services.AddScoped<AssigneeTypeApiClient>();
builder.Services.AddScoped<BusStationApiClient>();
builder.Services.AddScoped<WorkerApiClient>();
builder.Services.AddScoped<NoteTypeApiClient>();
builder.Services.AddScoped<LinkTypeApiClient>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
