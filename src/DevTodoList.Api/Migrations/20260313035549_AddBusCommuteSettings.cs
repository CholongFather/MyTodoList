using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBusCommuteSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusCommuteSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CommuteType = table.Column<int>(type: "INTEGER", nullable: false),
                    BusStopName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BusStopId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BusRouteNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DepartureTime = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    AlertMinutesBefore = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActiveDays = table.Column<int>(type: "INTEGER", nullable: false),
                    LastNotifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusCommuteSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusCommuteSettings_IsEnabled",
                table: "BusCommuteSettings",
                column: "IsEnabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusCommuteSettings");
        }
    }
}
