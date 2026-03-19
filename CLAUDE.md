# DevTodoList

## 프로젝트 구조
- `DevTodoList.Shared` - 공유 DTO/Enum/상수
- `DevTodoList.Api` - ASP.NET Core Web API (SQLite + EF Core)
- `DevTodoList.Web` - Blazor Server (MudBlazor UI)

## Tech Stack
- .NET 9, C#, MudBlazor 9.1.0
- SQLite + EF Core, Entity → Data/Entities/, Service → Services/

## 실행
- API: `dotnet run --project src/DevTodoList.Api` (포트 5100/5101)
- Web: `dotnet run --project src/DevTodoList.Web` (포트 5200/5201)
- 두 프로젝트 동시 실행 필요

## 빌드
- .sln 파일 없음 — 개별 프로젝트 빌드:
  ```bash
  dotnet build src/DevTodoList.Api/DevTodoList.Api.csproj
  dotnet build src/DevTodoList.Web/DevTodoList.Web.csproj
  ```

## 규칙
- 한글 주석 사용
- Entity → `Data/Entities/`, Service → `Services/`
- DTO는 Shared 프로젝트에 위치

## IIS 배포

MSBuild PreBuild/PostBuild 타겟으로 자동 배포 (`dotnet build -c Release`만 실행하면 됨):
```bash
dotnet build src/DevTodoList.Api/DevTodoList.Api.csproj -c Release
dotnet build src/DevTodoList.Web/DevTodoList.Web.csproj -c Release
```

배포 프로세스 (csproj에 내장):
1. `PrepareForBuild` 전: `app_offline.htm` → IIS 앱 종료 → DLL 잠금 해제 대기
2. `Build` 완료 → 빌드 결과가 `$(OutputPath)`에 직접 출력
3. `Build` 후: `app_offline.htm` 삭제 → IIS가 새 코드로 서비스 재개

IIS 물리 경로 = `$(OutputPath)` (bin/Release/net9.0/):
- API: `src/DevTodoList.Api/bin/Release/net9.0/` (http://localhost:5100)
- Web: `src/DevTodoList.Web/bin/Release/net9.0/` (http://localhost:5200)
- DB: `src/DevTodoList.Api/bin/Release/net9.0/devtodolist.db` (상대경로 `Data Source=devtodolist.db`)

주의사항:
- **InProcess 호스팅**: w3wp.exe가 DLL 잠금. PreBuild에서 app_offline으로 해제 대기
- Migration: `dotnet ef database update --project src/DevTodoList.Api -s src/DevTodoList.Api`
- IIS DB에 Migration 적용: `dotnet ef database update --project src/DevTodoList.Api -s src/DevTodoList.Api --connection "Data Source=C:/Repository/DevTodoList/src/DevTodoList.Api/bin/Release/net9.0/devtodolist.db"`

## DevTodoList API - 일정 자동 등록 가이드

사용자가 일정/TODO/할일/작업 목록을 공유하면, DevTodoList API를 통해 자동으로 등록한다.

### 프로세스
1. 프로젝트 확인: `GET http://localhost:5100/api/projects` → 기존 프로젝트 확인
2. 프로젝트 없으면 생성: `POST http://localhost:5100/api/projects`
3. 일정 등록 (두 가지 방법):
   - **방법 A (텍스트 파싱)**: `POST http://localhost:5100/api/schedule/create` - 자연어 텍스트를 파싱하여 TODO 일괄 생성
   - **방법 B (직접 생성)**: `POST http://localhost:5100/api/todos` - JSON 파일로 개별 생성 (한글 인코딩 문제 시 `--data-binary @file.json` 사용)

### API 엔드포인트

```bash
# 프로젝트 목록
GET http://localhost:5100/api/projects

# 프로젝트 생성
POST http://localhost:5100/api/projects
Content-Type: application/json; charset=utf-8
{"name":"프로젝트명","description":"설명","color":"#9C27B0"}

# 할일 생성 (개별)
POST http://localhost:5100/api/todos
Content-Type: application/json; charset=utf-8
{"title":"제목","description":"설명","projectId":1,"priority":2,"startDate":"2026-04-01","dueDate":"2026-04-15"}
# priority: 0=Low, 1=Normal, 2=High, 3=Urgent

# 일정 텍스트 일괄 생성
POST http://localhost:5100/api/schedule/create
Content-Type: application/json; charset=utf-8
{"text":"기획: ~ 2026. 4. 5.\n개발: 2026. 4. 5. ~ 2026. 4. 20.\nQA: 2026. 4. 20. ~ 2026. 4. 25.","projectId":1}

# 할일 목록 조회
GET http://localhost:5100/api/todos?projectId=1&status=0

# 할일 상태 변경
PATCH http://localhost:5100/api/todos/{id}/status
{"status":2}  # 0=Todo, 1=InProgress, 2=Done, 3=Archived

# 간트차트 데이터
GET http://localhost:5100/api/gantt?projectId=1
```

### 팀/작업분류/담당유형 필드

할일 생성 및 일정 등록 시 다음 필드를 추가로 사용한다.

| 필드 | 타입 | 설명 |
|---|---|---|
| teamId | long (nullable) | 담당 팀 ID (`GET /api/teams`로 조회) |
| workCategoryId | long (nullable) | 작업 분류 ID (팀 하위) |
| assigneeType | int | 0=Mine(내 작업), 1=Others(타인 작업) |

팀/작업분류는 DB에서 관리되며 설정 페이지에서 추가/수정/삭제 가능.
기본 시드 데이터:
- 기획팀: 기획
- 게토개발팀: 관리프로그램
- 디자인팀: 디자인, 퍼블리싱
- 서버개발팀: 서버, 웹
- 모바일개발팀: 앱

#### 예시: 팀/담당자 포함 할일 생성
```bash
POST http://localhost:5100/api/todos
Content-Type: application/json; charset=utf-8
{"title":"서버 API 개발","projectId":1,"priority":2,"teamId":4,"workCategoryId":5,"assigneeType":0,"startDate":"2026-04-01","dueDate":"2026-04-15"}
```

#### 예시: 기본 팀 지정 일정 텍스트 일괄 생성
```bash
POST http://localhost:5100/api/schedule/create
Content-Type: application/json; charset=utf-8
{"text":"기획: ~ 2026. 4. 5.\n개발: 2026. 4. 5. ~ 2026. 4. 20.","projectId":1,"defaultTeamId":4,"defaultWorkCategoryId":5,"defaultAssigneeType":0}
```

#### 팀 목록 조회
```bash
GET http://localhost:5100/api/teams
```

### 일정 팀별 구분 업로드 가이드

사용자가 여러 팀의 일정을 한번에 공유하면, 팀별로 구분하여 등록한다.

**프로세스:**
1. `GET http://localhost:5100/api/teams` → 팀 목록 및 작업분류 ID 확인
2. `GET http://localhost:5100/api/projects` → 프로젝트 ID 확인
3. 팀별로 할일을 분류한 뒤, 각 항목에 `teamId`, `workCategoryId`, `assigneeType` 지정
4. **방법 A (개별)**: `POST /api/todos`로 각 항목 생성
5. **방법 B (일괄)**: `POST /api/schedule/create`에 `defaultTeamId` 지정하여 일괄 생성

**assigneeType 판단 기준:**
- 내가 직접 수행하는 작업 → `0` (Mine)
- 다른 팀/사람이 수행하는 작업 → `1` (Others)
- Others 작업은 기한 초과 시 대시보드에서 "연장/완료" 프롬프트 표시

**예시 - 여러 팀 일정 한번에 등록:**
```
기획: ~ 4/5 → teamId=기획팀, assigneeType=1(Others)
디자인: 4/5 ~ 4/10 → teamId=디자인팀, assigneeType=1(Others)
서버 개발: 4/10 ~ 4/20 → teamId=서버개발팀, assigneeType=0(Mine)
QA: 4/20 ~ 4/25 → teamId=기획팀 or 별도, assigneeType=1(Others)
```

### 간트차트 계획 vs 실제

- 할일 생성 시 `startDate`/`endDate` → `plannedStartDate`/`plannedEndDate`로 자동 저장 (이후 변경 불가)
- 상태 → InProgress 전환 시 `actualStartDate` 자동 기록
- 상태 → Done 전환 시 `actualEndDate` 자동 기록
- 간트차트에서 계획(점선) / 실제(실선) 이중 바로 표시
- 실제가 계획보다 지연되면 빨간 지연 표시

### 한글 인코딩 처리
curl에서 한글 JSON 전송 시 `--data-binary @file.json` 방식 사용:
1. JSON 파일을 UTF-8로 생성 (Write 도구 사용)
2. `curl --data-binary @파일경로` 로 전송
3. 완료 후 임시 JSON 파일 삭제
