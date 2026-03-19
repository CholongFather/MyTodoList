using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>더미 시드 데이터 서비스</summary>
public static class SeedDataService
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // === 버스 정류장 마스터 시드 ===
        if (!await db.BusStations.AnyAsync())
        {
            db.BusStations.AddRange(
                new BusStationEntity
                {
                    Name = "청현마을·수원신갈IC - 1112 상행",
                    BusStopName = "청현마을·수원신갈IC",
                    BusRouteNumber = "1112",
                    GbisStationId = "228000700",
                    GbisRouteId = "234000016",
                    GbisStaOrder = 14,
                    Direction = 1, // 상행
                    SortOrder = 1
                },
                new BusStationEntity
                {
                    Name = "문정건영 - 1112 하행",
                    BusStopName = "문정건영",
                    BusRouteNumber = "1112",
                    GbisStationId = "123000014",
                    GbisRouteId = "234000016",
                    GbisStaOrder = 46,
                    Direction = 2, // 하행
                    SortOrder = 2
                },
                new BusStationEntity
                {
                    Name = "문정건영 - G6009 하행",
                    BusStopName = "문정건영",
                    BusRouteNumber = "G6009",
                    GbisStationId = "123000014",
                    GbisRouteId = "233000322",
                    GbisStaOrder = 34,
                    Direction = 2, // 하행
                    SortOrder = 3
                }
            );
            await db.SaveChangesAsync();
        }

        // 이미 데이터가 있으면 건너뜀
        if (await db.Projects.AnyAsync()) return;

        // === 팀/작업분류 시드 ===
        if (!await db.Teams.AnyAsync())
        {
            var teamPlanning = new TeamEntity { Name = "기획팀", Color = "#9C27B0", SortOrder = 1 };
            var teamGeto = new TeamEntity { Name = "게토개발팀", Color = "#FF9800", SortOrder = 2 };
            var teamDesign = new TeamEntity { Name = "디자인팀", Color = "#E91E63", SortOrder = 3 };
            var teamServer = new TeamEntity { Name = "서버개발팀", Color = "#1976D2", SortOrder = 4 };
            var teamMobile = new TeamEntity { Name = "모바일개발팀", Color = "#4CAF50", SortOrder = 5 };
            db.Teams.AddRange(teamPlanning, teamGeto, teamDesign, teamServer, teamMobile);
            await db.SaveChangesAsync();

            db.WorkCategories.AddRange(
                new WorkCategoryEntity { Name = "기획", TeamId = teamPlanning.Id, SortOrder = 1 },
                new WorkCategoryEntity { Name = "관리프로그램", TeamId = teamGeto.Id, SortOrder = 1 },
                new WorkCategoryEntity { Name = "디자인", TeamId = teamDesign.Id, SortOrder = 1 },
                new WorkCategoryEntity { Name = "퍼블리싱", TeamId = teamDesign.Id, SortOrder = 2 },
                new WorkCategoryEntity { Name = "서버", TeamId = teamServer.Id, SortOrder = 1 },
                new WorkCategoryEntity { Name = "웹", TeamId = teamServer.Id, SortOrder = 2 },
                new WorkCategoryEntity { Name = "앱", TeamId = teamMobile.Id, SortOrder = 1 }
            );
            await db.SaveChangesAsync();
        }

        // === 태그 생성 ===
        var tagDev = new TagEntity { Name = "개발", Color = "#1976D2" };
        var tagDesign = new TagEntity { Name = "디자인", Color = "#9C27B0" };
        var tagQA = new TagEntity { Name = "QA", Color = "#FF9800" };
        var tagApi = new TagEntity { Name = "API", Color = "#4CAF50" };
        var tagFrontend = new TagEntity { Name = "프론트", Color = "#00BCD4" };
        var tagUrgent = new TagEntity { Name = "긴급", Color = "#F44336" };
        db.Tags.AddRange(tagDev, tagDesign, tagQA, tagApi, tagFrontend, tagUrgent);
        await db.SaveChangesAsync();

        // === 프로젝트 1: 게임 허브 매장 지면 ===
        var proj1 = new ProjectEntity
        {
            Name = "게임 허브 매장 지면",
            Description = "허브 내 일부 영역을 매장 지면으로 활용(매장 허브). 게임 코드 기반 광고 지면 출력 및 인디케이터 개선.",
            Color = "#1976D2",
            SortOrder = 1
        };
        db.Projects.Add(proj1);
        await db.SaveChangesAsync();

        var todos1 = new List<TodoItemEntity>
        {
            new() { Title = "디자인", ProjectId = proj1.Id, StartDate = D(2026,1,15), EndDate = D(2026,2,11), DueDate = D(2026,2,11), Status = 2, Priority = 1, SortOrder = 1 },
            new() { Title = "마크업", ProjectId = proj1.Id, StartDate = D(2026,2,12), EndDate = D(2026,2,23), DueDate = D(2026,2,23), Status = 2, Priority = 1, SortOrder = 2 },
            new() { Title = "[개발] API", ProjectId = proj1.Id, StartDate = D(2026,2,24), EndDate = D(2026,3,4), DueDate = D(2026,3,4), Status = 1, Priority = 2, SortOrder = 3 },
            new() { Title = "[개발] 개발연동 테스트", ProjectId = proj1.Id, StartDate = D(2026,3,4), EndDate = D(2026,3,5), DueDate = D(2026,3,5), Status = 0, Priority = 2, SortOrder = 4 },
            new() { Title = "[개발] 프론트", ProjectId = proj1.Id, StartDate = D(2026,2,24), EndDate = D(2026,3,6), DueDate = D(2026,3,6), Status = 1, Priority = 2, SortOrder = 5 },
            new() { Title = "[QA] 내부 테스트", ProjectId = proj1.Id, StartDate = D(2026,3,9), EndDate = D(2026,3,13), DueDate = D(2026,3,13), Status = 0, Priority = 1, SortOrder = 6 },
            new() { Title = "LIVE 배포", ProjectId = proj1.Id, StartDate = D(2026,3,31), EndDate = D(2026,3,31), DueDate = D(2026,3,31), Status = 0, Priority = 3, SortOrder = 7 },
        };
        db.TodoItems.AddRange(todos1);
        await db.SaveChangesAsync();

        // 체크리스트 추가
        var apiTodo = todos1[2];
        db.TodoCheckItems.AddRange(
            new TodoCheckItemEntity { Title = "카운터PC - 허브에 출력되는 매장 지면 설정 API", TodoItemId = apiTodo.Id, SortOrder = 1 },
            new TodoCheckItemEntity { Title = "손님PC - 매장 허브 지면 우선 출력 API", TodoItemId = apiTodo.Id, SortOrder = 2 },
            new TodoCheckItemEntity { Title = "Admin - 지면 태그명 컬럼 추가 API", TodoItemId = apiTodo.Id, SortOrder = 3 },
            new TodoCheckItemEntity { Title = "AdDive - 게임 코드 기준 광고 지면 설정 API", TodoItemId = apiTodo.Id, SortOrder = 4, IsCompleted = true }
        );

        // 링크 추가
        db.TodoLinks.AddRange(
            new TodoLinkEntity { Title = "기획서 (Confluence)", Url = "https://confluence.example.com/gamehub-store", LinkType = 1, TodoItemId = todos1[0].Id },
            new TodoLinkEntity { Title = "JIRA 에픽", Url = "https://jira.example.com/GAMEHUB-123", LinkType = 0, TodoItemId = apiTodo.Id }
        );

        // 태그 연결
        db.TodoTags.AddRange(
            new TodoTagEntity { TodoItemId = todos1[0].Id, TagId = tagDesign.Id },
            new TodoTagEntity { TodoItemId = todos1[2].Id, TagId = tagApi.Id },
            new TodoTagEntity { TodoItemId = todos1[2].Id, TagId = tagDev.Id },
            new TodoTagEntity { TodoItemId = todos1[4].Id, TagId = tagFrontend.Id },
            new TodoTagEntity { TodoItemId = todos1[4].Id, TagId = tagDev.Id },
            new TodoTagEntity { TodoItemId = todos1[5].Id, TagId = tagQA.Id },
            new TodoTagEntity { TodoItemId = todos1[6].Id, TagId = tagUrgent.Id }
        );
        await db.SaveChangesAsync();

        // === 프로젝트 2: 이벤트 프로모션 ===
        var proj2 = new ProjectEntity
        {
            Name = "이벤트 프로모션",
            Description = "이벤트 빌더 연동 프로모션 개발. 라이브 데이터 환경 API 연동.",
            Color = "#FF9800",
            SortOrder = 2
        };
        db.Projects.Add(proj2);
        await db.SaveChangesAsync();

        var todos2 = new List<TodoItemEntity>
        {
            new() { Title = "기획", ProjectId = proj2.Id, StartDate = D(2026,2,20), EndDate = D(2026,3,5), DueDate = D(2026,3,5), Status = 2, Priority = 1, SortOrder = 1 },
            new() { Title = "디자인", ProjectId = proj2.Id, StartDate = D(2026,3,1), EndDate = D(2026,3,6), DueDate = D(2026,3,6), Status = 2, Priority = 1, SortOrder = 2 },
            new() { Title = "마크업", ProjectId = proj2.Id, StartDate = D(2026,3,6), EndDate = D(2026,3,10), DueDate = D(2026,3,10), Status = 1, Priority = 1, SortOrder = 3 },
            new() { Title = "[NMP] 게토개발팀", ProjectId = proj2.Id, StartDate = D(2026,3,9), EndDate = D(2026,3,17), DueDate = D(2026,3,17), Status = 1, Priority = 2, SortOrder = 4 },
            new() { Title = "[NMP] 서비스개발팀", ProjectId = proj2.Id, StartDate = D(2026,3,9), EndDate = D(2026,3,27), DueDate = D(2026,3,27), Status = 1, Priority = 2, SortOrder = 5 },
            new() { Title = "[이벤트빌더] API 명세", ProjectId = proj2.Id, StartDate = D(2026,3,16), EndDate = D(2026,3,18), DueDate = D(2026,3,18), Status = 0, Priority = 2, SortOrder = 6 },
            new() { Title = "[이벤트빌더] 더미 데이터 연동", ProjectId = proj2.Id, StartDate = D(2026,3,18), EndDate = D(2026,3,27), DueDate = D(2026,3,27), Status = 0, Priority = 2, SortOrder = 7 },
            new() { Title = "[이벤트빌더] 라이브 데이터 환경 API 제공", ProjectId = proj2.Id, StartDate = D(2026,4,1), EndDate = D(2026,4,10), DueDate = D(2026,4,10), Status = 0, Priority = 3, SortOrder = 8 },
            new() { Title = "개발 테스트", ProjectId = proj2.Id, StartDate = D(2026,3,30), EndDate = D(2026,3,31), DueDate = D(2026,3,31), Status = 0, Priority = 2, SortOrder = 9 },
            new() { Title = "[QA] 내부 QA", ProjectId = proj2.Id, StartDate = D(2026,4,1), EndDate = D(2026,4,16), DueDate = D(2026,4,16), Status = 0, Priority = 1, SortOrder = 10 },
            new() { Title = "[QA] 라이브 QA", ProjectId = proj2.Id, StartDate = D(2026,4,17), EndDate = D(2026,4,20), DueDate = D(2026,4,20), Status = 0, Priority = 2, SortOrder = 11 },
            new() { Title = "프로모션 시작", ProjectId = proj2.Id, StartDate = D(2026,4,23), EndDate = D(2026,4,23), DueDate = D(2026,4,23), Status = 0, Priority = 3, SortOrder = 12 },
        };
        db.TodoItems.AddRange(todos2);
        await db.SaveChangesAsync();

        // 태그 연결
        db.TodoTags.AddRange(
            new TodoTagEntity { TodoItemId = todos2[1].Id, TagId = tagDesign.Id },
            new TodoTagEntity { TodoItemId = todos2[3].Id, TagId = tagDev.Id },
            new TodoTagEntity { TodoItemId = todos2[4].Id, TagId = tagDev.Id },
            new TodoTagEntity { TodoItemId = todos2[5].Id, TagId = tagApi.Id },
            new TodoTagEntity { TodoItemId = todos2[7].Id, TagId = tagApi.Id },
            new TodoTagEntity { TodoItemId = todos2[7].Id, TagId = tagUrgent.Id },
            new TodoTagEntity { TodoItemId = todos2[9].Id, TagId = tagQA.Id },
            new TodoTagEntity { TodoItemId = todos2[10].Id, TagId = tagQA.Id },
            new TodoTagEntity { TodoItemId = todos2[11].Id, TagId = tagUrgent.Id }
        );

        // 체크리스트
        db.TodoCheckItems.AddRange(
            new TodoCheckItemEntity { Title = "이벤트 빌더 일정 변경 요청 확인", TodoItemId = todos2[5].Id, SortOrder = 1, IsCompleted = true },
            new TodoCheckItemEntity { Title = "라이브 데이터 API 4/10 제공 일정 확인", TodoItemId = todos2[7].Id, SortOrder = 1 },
            new TodoCheckItemEntity { Title = "일정 내 프로모션 시작 가능 여부 확인", TodoItemId = todos2[7].Id, SortOrder = 2 },
            new TodoCheckItemEntity { Title = "2시 회의 일정 조율", TodoItemId = todos2[7].Id, SortOrder = 3, IsCompleted = true }
        );

        // 링크
        db.TodoLinks.AddRange(
            new TodoLinkEntity { Title = "이벤트 프로모션 기획서", Url = "https://confluence.example.com/event-promo", LinkType = 1, TodoItemId = todos2[0].Id },
            new TodoLinkEntity { Title = "이벤트빌더 API 명세", Url = "https://confluence.example.com/event-builder-api", LinkType = 1, TodoItemId = todos2[5].Id }
        );
        await db.SaveChangesAsync();

        // === 프로젝트 3: NMP 인증 개선 ===
        var proj3 = new ProjectEntity
        {
            Name = "NMP 인증 개선",
            Description = "NMP.Auth 로그인 플로우 개선 및 보안 강화",
            Color = "#4CAF50",
            SortOrder = 3
        };
        db.Projects.Add(proj3);
        await db.SaveChangesAsync();

        db.TodoItems.AddRange(
            new TodoItemEntity { Title = "로그인 플로우 분석", ProjectId = proj3.Id, StartDate = D(2026,3,10), EndDate = D(2026,3,14), DueDate = D(2026,3,14), Status = 0, Priority = 1, SortOrder = 1 },
            new TodoItemEntity { Title = "JWT 토큰 갱신 로직 개선", ProjectId = proj3.Id, StartDate = D(2026,3,14), EndDate = D(2026,3,21), DueDate = D(2026,3,21), Status = 0, Priority = 2, SortOrder = 2 },
            new TodoItemEntity { Title = "세션 타임아웃 처리 개선", ProjectId = proj3.Id, StartDate = D(2026,3,17), EndDate = D(2026,3,24), DueDate = D(2026,3,24), Status = 0, Priority = 1, SortOrder = 3 }
        );
        await db.SaveChangesAsync();
    }

    private static DateTime D(int y, int m, int d) => new(y, m, d, 0, 0, 0, DateTimeKind.Utc);
}
