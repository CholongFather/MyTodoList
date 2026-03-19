using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevTodoList.Api.Data.Entities;

/// <summary>버스 정류장 마스터 (설정에서 관리)</summary>
[Table("BusStations")]
public class BusStationEntity : EntityBase
{
    /// <summary>표시 이름 (예: "청현마을·수원신갈IC - 1112 상행")</summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>정류장 이름</summary>
    [Required, MaxLength(200)]
    public string BusStopName { get; set; } = string.Empty;

    /// <summary>노선 번호</summary>
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

    /// <summary>진행 방향 (0=미지정, 1=상행, 2=하행)</summary>
    public int Direction { get; set; }

    /// <summary>정렬 순서</summary>
    public int SortOrder { get; set; }

    /// <summary>이 정류장을 사용하는 출퇴근 설정들</summary>
    public ICollection<BusCommuteSettingEntity> CommuteSettings { get; set; } = [];
}
