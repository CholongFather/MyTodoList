using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceDepartureTimeWithMonitoringWindow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartureTime",
                table: "BusCommuteSettings",
                newName: "MonitoringStartTime");

            migrationBuilder.AddColumn<string>(
                name: "MonitoringEndTime",
                table: "BusCommuteSettings",
                type: "TEXT",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            // 기존 DepartureTime(→MonitoringStartTime) 기반 데이터 변환
            // MonitoringEndTime = 기존 출발시간 + 5분 (HH:mm 형식)
            migrationBuilder.Sql(
                "UPDATE BusCommuteSettings SET MonitoringEndTime = strftime('%H:%M', MonitoringStartTime, '+5 minutes');");
            // MonitoringStartTime = 기존 출발시간 - 30분 (HH:mm 형식)
            migrationBuilder.Sql(
                "UPDATE BusCommuteSettings SET MonitoringStartTime = strftime('%H:%M', MonitoringStartTime, '-30 minutes');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonitoringEndTime",
                table: "BusCommuteSettings");

            migrationBuilder.RenameColumn(
                name: "MonitoringStartTime",
                table: "BusCommuteSettings",
                newName: "DepartureTime");
        }
    }
}
