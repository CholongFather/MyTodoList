using DevTodoList.Api.Data;
using DevTodoList.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// SQLite + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 서비스 등록
builder.Services.AddDevTodoServices();
builder.Services.AddHttpClient();

// 버스 알림 백그라운드 서비스
builder.Services.AddHostedService<BusNotificationBackgroundService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - Blazor Web에서 호출 허용
var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>()
    ?? ["https://localhost:5201", "http://localhost:5200"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWeb", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// DB 자동 마이그레이션 + 시드 데이터
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await SeedDataService.SeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowBlazorWeb");
app.MapControllers();

app.Run();
