using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAssigneeAndPlannedDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEndDate",
                table: "TodoItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStartDate",
                table: "TodoItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssigneeType",
                table: "TodoItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedEndDate",
                table: "TodoItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedStartDate",
                table: "TodoItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeamId",
                table: "TodoItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WorkCategoryId",
                table: "TodoItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamId = table.Column<long>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkCategories_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 기존 데이터 backfill: StartDate/EndDate → PlannedStartDate/PlannedEndDate
            migrationBuilder.Sql(@"
                UPDATE TodoItems
                SET PlannedStartDate = StartDate,
                    PlannedEndDate = COALESCE(EndDate, DueDate)
                WHERE StartDate IS NOT NULL OR EndDate IS NOT NULL OR DueDate IS NOT NULL;
            ");

            // 이미 Done 상태인 항목에 ActualEndDate = CompletedAt 설정
            migrationBuilder.Sql(@"
                UPDATE TodoItems
                SET ActualEndDate = CompletedAt
                WHERE Status = 2 AND CompletedAt IS NOT NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_AssigneeType",
                table: "TodoItems",
                column: "AssigneeType");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_WorkCategoryId",
                table: "TodoItems",
                column: "WorkCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkCategories_TeamId",
                table: "WorkCategories",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Teams_TeamId",
                table: "TodoItems",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_WorkCategories_WorkCategoryId",
                table: "TodoItems",
                column: "WorkCategoryId",
                principalTable: "WorkCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Teams_TeamId",
                table: "TodoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_WorkCategories_WorkCategoryId",
                table: "TodoItems");

            migrationBuilder.DropTable(
                name: "WorkCategories");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_AssigneeType",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_WorkCategoryId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "ActualEndDate",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "ActualStartDate",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "AssigneeType",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "PlannedEndDate",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "PlannedStartDate",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "WorkCategoryId",
                table: "TodoItems");
        }
    }
}
