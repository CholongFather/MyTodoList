namespace DevTodoList.Shared.Helpers;

/// <summary>
/// 타인 작업(Others) 상태를 날짜 기반으로 자동 계산
/// - 시작일 전 → 할일
/// - 시작일~종료일 사이 → 진행중
/// - 마감일 지남 & 종료일 미도래 → 지연
/// - 종료일 지남 & 종료일 > 마감일 → 지연완료
/// - 종료일 지남 & 종료일 < 마감일 → 조기완료
/// - 종료일 지남 & 종료일 == 마감일 → 완료
/// </summary>
public static class OthersStatusHelper
{
    public enum ComputedStatus
    {
        /// <summary>시작일 전</summary>
        Todo,
        /// <summary>진행 중</summary>
        InProgress,
        /// <summary>마감일 지남, 아직 종료 안 됨</summary>
        Overdue,
        /// <summary>정상 완료 (종료일 = 마감일)</summary>
        Done,
        /// <summary>조기 완료 (종료일 < 마감일)</summary>
        EarlyDone,
        /// <summary>지연 완료 (종료일 > 마감일)</summary>
        LateDone
    }

    /// <summary>날짜 기반 상태 계산</summary>
    public static ComputedStatus Compute(DateTime? startDate, DateTime? endDate, DateTime? dueDate, DateTime? today = null)
    {
        var now = (today ?? DateTime.Today).Date;
        var start = startDate?.Date;
        var end = endDate?.Date ?? dueDate?.Date; // 종료일 없으면 마감일 사용
        var due = dueDate?.Date ?? endDate?.Date; // 마감일 없으면 종료일 사용

        // 날짜 없으면 기본 할일
        if (start is null && end is null) return ComputedStatus.Todo;

        // 시작일 전
        if (start.HasValue && now < start.Value) return ComputedStatus.Todo;

        // 종료일 지남
        if (end.HasValue && now >= end.Value)
        {
            if (due.HasValue)
            {
                if (end.Value < due.Value) return ComputedStatus.EarlyDone;
                if (end.Value > due.Value) return ComputedStatus.LateDone;
            }
            return ComputedStatus.Done;
        }

        // 마감일 지남인데 종료일 미도래
        if (due.HasValue && now > due.Value) return ComputedStatus.Overdue;

        // 시작일~종료일 사이
        return ComputedStatus.InProgress;
    }

    /// <summary>상태 라벨</summary>
    public static string ToLabel(ComputedStatus status) => status switch
    {
        ComputedStatus.Todo => "할일",
        ComputedStatus.InProgress => "진행중",
        ComputedStatus.Overdue => "지연",
        ComputedStatus.Done => "완료",
        ComputedStatus.EarlyDone => "조기완료",
        ComputedStatus.LateDone => "지연완료",
        _ => status.ToString()
    };

    /// <summary>MudBlazor 색상 문자열</summary>
    public static string ToColor(ComputedStatus status) => status switch
    {
        ComputedStatus.Todo => "Default",
        ComputedStatus.InProgress => "Info",
        ComputedStatus.Overdue => "Error",
        ComputedStatus.Done => "Success",
        ComputedStatus.EarlyDone => "Tertiary",
        ComputedStatus.LateDone => "Warning",
        _ => "Default"
    };
}
