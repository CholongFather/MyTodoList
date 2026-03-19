using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBusStations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BusStationId",
                table: "BusCommuteSettings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusStations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BusStopName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BusRouteNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    GbisStationId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    GbisRouteId = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    GbisStaOrder = table.Column<int>(type: "INTEGER", nullable: true),
                    Direction = table.Column<int>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusStations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusCommuteSettings_BusStationId",
                table: "BusCommuteSettings",
                column: "BusStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusCommuteSettings_BusStations_BusStationId",
                table: "BusCommuteSettings",
                column: "BusStationId",
                principalTable: "BusStations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusCommuteSettings_BusStations_BusStationId",
                table: "BusCommuteSettings");

            migrationBuilder.DropTable(
                name: "BusStations");

            migrationBuilder.DropIndex(
                name: "IX_BusCommuteSettings_BusStationId",
                table: "BusCommuteSettings");

            migrationBuilder.DropColumn(
                name: "BusStationId",
                table: "BusCommuteSettings");
        }
    }
}
