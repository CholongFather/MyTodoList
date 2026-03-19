using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerIsMe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMe",
                table: "Workers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMe",
                table: "Workers");
        }
    }
}
