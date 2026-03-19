using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAssigneeTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssigneeTypeId",
                table: "TodoItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssigneeTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IsMine = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssigneeTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_AssigneeTypeId",
                table: "TodoItems",
                column: "AssigneeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_AssigneeTypes_AssigneeTypeId",
                table: "TodoItems",
                column: "AssigneeTypeId",
                principalTable: "AssigneeTypes",
                principalColumn: "Id");

            // 기본 담당 유형 시딩
            migrationBuilder.Sql("""
                INSERT INTO AssigneeTypes (Name, Color, IsMine, SortOrder, CreatedAt)
                VALUES ('내 작업', '#1976D2', 1, 0, datetime('now'));
                INSERT INTO AssigneeTypes (Name, Color, IsMine, SortOrder, CreatedAt)
                VALUES ('타인 작업', '#FF9800', 0, 1, datetime('now'));
                """);

            // 기존 TodoItems 데이터 매핑
            migrationBuilder.Sql("""
                UPDATE TodoItems SET AssigneeTypeId = (SELECT Id FROM AssigneeTypes WHERE IsMine = 1 LIMIT 1)
                WHERE AssigneeType = 0 AND AssigneeTypeId IS NULL;
                UPDATE TodoItems SET AssigneeTypeId = (SELECT Id FROM AssigneeTypes WHERE IsMine = 0 LIMIT 1)
                WHERE AssigneeType = 1 AND AssigneeTypeId IS NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_AssigneeTypes_AssigneeTypeId",
                table: "TodoItems");

            migrationBuilder.DropTable(
                name: "AssigneeTypes");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_AssigneeTypeId",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "AssigneeTypeId",
                table: "TodoItems");
        }
    }
}
