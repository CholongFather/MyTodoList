using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerNoteTypeLinkType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LinkTypeId",
                table: "TodoLinks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NoteTypeId",
                table: "CaseNotes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LinkTypeId",
                table: "CaseLinks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LinkTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoteTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TodoWorkers",
                columns: table => new
                {
                    TodoItemId = table.Column<long>(type: "INTEGER", nullable: false),
                    WorkerId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TodoWorkers", x => new { x.TodoItemId, x.WorkerId });
                    table.ForeignKey(
                        name: "FK_TodoWorkers_TodoItems_TodoItemId",
                        column: x => x.TodoItemId,
                        principalTable: "TodoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TodoWorkers_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TodoLinks_LinkTypeId",
                table: "TodoLinks",
                column: "LinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseNotes_NoteTypeId",
                table: "CaseNotes",
                column: "NoteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseLinks_LinkTypeId",
                table: "CaseLinks",
                column: "LinkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoWorkers_WorkerId",
                table: "TodoWorkers",
                column: "WorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_Name",
                table: "Workers",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseLinks_LinkTypes_LinkTypeId",
                table: "CaseLinks",
                column: "LinkTypeId",
                principalTable: "LinkTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseNotes_NoteTypes_NoteTypeId",
                table: "CaseNotes",
                column: "NoteTypeId",
                principalTable: "NoteTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLinks_LinkTypes_LinkTypeId",
                table: "TodoLinks",
                column: "LinkTypeId",
                principalTable: "LinkTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseLinks_LinkTypes_LinkTypeId",
                table: "CaseLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseNotes_NoteTypes_NoteTypeId",
                table: "CaseNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoLinks_LinkTypes_LinkTypeId",
                table: "TodoLinks");

            migrationBuilder.DropTable(
                name: "LinkTypes");

            migrationBuilder.DropTable(
                name: "NoteTypes");

            migrationBuilder.DropTable(
                name: "TodoWorkers");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_TodoLinks_LinkTypeId",
                table: "TodoLinks");

            migrationBuilder.DropIndex(
                name: "IX_CaseNotes_NoteTypeId",
                table: "CaseNotes");

            migrationBuilder.DropIndex(
                name: "IX_CaseLinks_LinkTypeId",
                table: "CaseLinks");

            migrationBuilder.DropColumn(
                name: "LinkTypeId",
                table: "TodoLinks");

            migrationBuilder.DropColumn(
                name: "NoteTypeId",
                table: "CaseNotes");

            migrationBuilder.DropColumn(
                name: "LinkTypeId",
                table: "CaseLinks");
        }
    }
}
