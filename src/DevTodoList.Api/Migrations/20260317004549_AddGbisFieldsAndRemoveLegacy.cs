using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGbisFieldsAndRemoveLegacy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusStopId",
                table: "BusCommuteSettings");

            migrationBuilder.RenameColumn(
                name: "KakaoApiKey",
                table: "NotificationSettings",
                newName: "GbisApiKey");

            migrationBuilder.AddColumn<string>(
                name: "GbisRouteId",
                table: "BusCommuteSettings",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GbisStaOrder",
                table: "BusCommuteSettings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GbisStationId",
                table: "BusCommuteSettings",
                type: "TEXT",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GbisRouteId",
                table: "BusCommuteSettings");

            migrationBuilder.DropColumn(
                name: "GbisStaOrder",
                table: "BusCommuteSettings");

            migrationBuilder.DropColumn(
                name: "GbisStationId",
                table: "BusCommuteSettings");

            migrationBuilder.RenameColumn(
                name: "GbisApiKey",
                table: "NotificationSettings",
                newName: "KakaoApiKey");

            migrationBuilder.AddColumn<string>(
                name: "BusStopId",
                table: "BusCommuteSettings",
                type: "TEXT",
                maxLength: 50,
                nullable: true);
        }
    }
}
