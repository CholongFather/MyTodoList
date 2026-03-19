using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevTodoList.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWebhookUrlsToSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusWebhookUrl",
                table: "NotificationSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduleWebhookUrl",
                table: "NotificationSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "NotificationSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "BusWebhookUrl", "ScheduleWebhookUrl" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusWebhookUrl",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "ScheduleWebhookUrl",
                table: "NotificationSettings");
        }
    }
}
