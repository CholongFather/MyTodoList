using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkTypeUrlPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlPattern",
                table: "LinkTypes",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlPattern",
                table: "LinkTypes");
        }
    }
}
