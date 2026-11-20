using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsArchivedWithStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "Projects",
                newName: "Status");

            // IsArchived=1(true) → Status=2(Archived) 데이터 마이그레이션
            migrationBuilder.Sql("UPDATE Projects SET Status = 2 WHERE Status = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Projects",
                newName: "IsArchived");
        }
    }
}
