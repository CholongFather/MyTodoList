# DevTodoList

## 구조
```
DevTodoList.Shared  - DTO, Enum, Helper, Constants
DevTodoList.Api     - ASP.NET Core Web API (SQLite + EF Core)
DevTodoList.Web     - Blazor Server (MudBlazor 9.1.0)
```

## Tech
.NET 9, C#, MudBlazor 9.1.0, SQLite + EF Core, 한글 주석

## 빌드 & 실행
```bash
# 빌드 (.sln 없음, 개별 csproj)
dotnet build src/DevTodoList.Api/DevTodoList.Api.csproj
dotnet build src/DevTodoList.Web/DevTodoList.Web.csproj

# 실행 (두 프로젝트 동시)
dotnet run --project src/DevTodoList.Api   # :5100
dotnet run --project src/DevTodoList.Web   # :5200
```

## IIS 배포
```bash
dotnet build src/DevTodoList.Api/DevTodoList.Api.csproj -c Release
dotnet build src/DevTodoList.Web/DevTodoList.Web.csproj -c Release
```
- IIS 물리경로 = `bin/Release/net9.0/` (API :5100, Web :5200)
- DB: `src/DevTodoList.Api/bin/Release/net9.0/devtodolist.db`
- InProcess 호스팅 → csproj PreBuild/PostBuild가 `app_offline.htm` 자동 관리
- Migration: `dotnet ef database update --project src/DevTodoList.Api -s src/DevTodoList.Api --connection "Data Source=C:/Repository/DevTodoList/src/DevTodoList.Api/bin/Release/net9.0/devtodolist.db"`

## 코드 규칙
- Entity → `Api/Data/Entities/`, Service → `Api/Services/`, DTO → `Shared/DTOs/`
- API Client → `Web/Services/ApiClient/` (모든 mutation에 `EnsureSuccessOrThrowAsync()` 필수)
- Razor Page → `Web/Components/Pages/{기능}/`

## 주요 기능 & 아키텍처

### 할일 (Todos)
- CRUD + 자동저장, 태그/작업자 다중 지정 (칩 토글)
- 상태: Todo(0) → InProgress(1) → Done(2) → Archived(3)
- **Others 타입**: `OthersStatusHelper.Compute()`로 날짜 기반 자동 상태 계산 (계획일 무관, 실제일만 사용)
- 계획일(PlannedStart/End) vs 실제일(Start/End/Due) vs 자동기록(ActualStart/End)

### 케이스 (Cases)
- 이슈 트래킹. 노트/링크 하위 CRUD
- 노트 작성자: IsMe 작업자 이름 자동 설정

### 간트차트
- JS interop (`gantt.js`), 오늘 기준 자동 스크롤
- 계획(점선) / 실제(실선) 이중 바 표시

### 버스 알림 (BusNotificationBackgroundService)
- 30초 주기 GBIS 실시간 도착정보 조회
- **임계값별 캐싱**: AlertThresholds "15,10,5" → 각 임계값마다 1회 알림 (캐시키: `설정ID_임계값`)
- 공휴일/연차 스킵, 오늘 넘기기(LastNotifiedAt 자정)
- Teams Webhook (Adaptive Card)으로 발송
- DateTime.Now 사용 (모니터링 윈도우가 로컬 시간 기준)

### 설정 페이지 (10개 서브페이지)
tags, teams, environments, workers, note-types, link-types, bus-stations, holidays, notifications, assignee-types

## API 라우트 요약
| 리소스 | 경로 |
|---|---|
| 프로젝트 | `api/projects` |
| 할일 | `api/todos` |
| 케이스 | `api/cases` |
| 간트 | `api/gantt` |
| 대시보드 | `api/dashboard` |
| 일정 파싱 | `api/schedule` |
| 팀 | `api/teams` |
| 태그 | `api/tags` |
| 작업자 | `api/workers` |
| 버스 통근 | `api/bus-commute` |
| 버스 정류장 | `api/bus-stations` |
| 공휴일 | `api/holidays` |
| 링크 유형 | `api/link-types` |
| 노트 유형 | `api/note-types` |
| 환경 | `api/environments` |
| 담당유형 | `api/assignee-types` |
| 설정 | `api/settings` |

## 일정 등록 가이드

사용자가 일정을 공유하면 API로 자동 등록:
1. `GET /api/projects` → 프로젝트 확인/생성
2. `GET /api/teams` → 팀/작업분류 ID 확인
3. 등록: `POST /api/schedule/create` (텍스트 파싱) 또는 `POST /api/todos` (개별)

```bash
# 일괄 생성
POST http://localhost:5100/api/schedule/create
{"text":"기획: ~ 4. 5.\n개발: 4. 5. ~ 4. 20.","projectId":1,"defaultTeamId":4,"defaultAssigneeType":0}

# 개별 생성
POST http://localhost:5100/api/todos
{"title":"제목","projectId":1,"priority":2,"teamId":4,"assigneeType":0,"startDate":"2026-04-01","dueDate":"2026-04-15"}
```

- priority: 0=Low, 1=Normal, 2=High, 3=Urgent
- assigneeType: 0=Mine, 1=Others
- 한글 curl: `--data-binary @file.json` (UTF-8 파일)
