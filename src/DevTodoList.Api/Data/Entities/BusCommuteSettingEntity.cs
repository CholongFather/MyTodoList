using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevTodoList.Shared.Constants;

namespace DevTodoList.Api.Data.Entities;

/// <summary>버스 출퇴근 알림 설정</summary>
[Table("BusCommuteSettings")]
public class BusCommuteSettingEntity : EntityBase
{
    /// <summary>설정 이름 (예: "평일 출근")</summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>출퇴근 구분: Morning=0, Evening=1</summary>
    public int CommuteType { get; set; }

    /// <summary>버스 정류장 이름</summary>
    [Required, MaxLength(200)]
    public string BusStopName { get; set; } = string.Empty;

    /// <summary>버스 노선 번호</summary>
    [Required, MaxLength(50)]
    public string BusRouteNumber { get; set; } = string.Empty;

    /// <summary>GBIS 정류소 ID</summary>
    [MaxLength(20)]
    public string? GbisStationId { get; set; }

    /// <summary>GBIS 노선 ID</summary>
    [MaxLength(20)]
    public string? GbisRouteId { get; set; }

    /// <summary>GBIS 정류소 순번</summary>
    public int? GbisStaOrder { get; set; }

    /// <summary>모니터링 시작 시간 (HH:mm)</summary>
    [Required, MaxLength(5)]
    public string MonitoringStartTime { get; set; } = BusCommuteDefaults.MonitoringStartTime;

    /// <summary>모니터링 종료 시간 (HH:mm)</summary>
    [Required, MaxLength(5)]
    public string MonitoringEndTime { get; set; } = BusCommuteDefaults.MonitoringEndTime;

    /// <summary>버스 도착 N분 전 알림</summary>
    public int AlertMinutesBefore { get; set; } = BusCommuteDefaults.AlertMinutesBefore;

    /// <summary>다단계 알림 임계값 (분, 쉼표 구분, 내림차순. 예: "15,10,5")</summary>
    [MaxLength(50)]
    public string AlertThresholds { get; set; } = BusCommuteDefaults.AlertThresholds;

    /// <summary>활성화 여부</summary>
    public bool IsEnabled { get; set; } = BusCommuteDefaults.IsEnabled;

    /// <summary>적용 요일 비트 플래그</summary>
    public int ActiveDays { get; set; } = BusCommuteDefaults.ActiveDays;

    /// <summary>마지막 알림 발송 시각 (중복 방지)</summary>
    public DateTime? LastNotifiedAt { get; set; }

    /// <summary>정류장 마스터 FK (설정 시 GBIS 필드는 마스터에서 해석)</summary>
    public long? BusStationId { get; set; }

    [ForeignKey(nameof(BusStationId))]
    public BusStationEntity? BusStation { get; set; }
}
