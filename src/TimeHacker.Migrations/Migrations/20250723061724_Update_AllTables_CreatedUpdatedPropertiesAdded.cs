using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Update_AllTables_CreatedUpdatedPropertiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdateTimestamp",
                table: "ScheduleSnapshot");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTimestamp",
                table: "User",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Tag",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTimestamp",
                table: "Tag",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTimestamp",
                table: "ScheduleEntity",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "ScheduledTask",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "ScheduledCategory",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTimestamp",
                table: "FixedTask",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTimestamp",
                table: "DynamicTask",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTimestamp",
                table: "Category",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTimestamp",
                table: "Category",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UpdatedTimestamp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UpdatedTimestamp",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "UpdatedTimestamp",
                table: "ScheduleEntity");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "ScheduledTask");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "ScheduledCategory");

            migrationBuilder.DropColumn(
                name: "UpdatedTimestamp",
                table: "FixedTask");

            migrationBuilder.DropColumn(
                name: "UpdatedTimestamp",
                table: "DynamicTask");

            migrationBuilder.DropColumn(
                name: "CreatedTimestamp",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "UpdatedTimestamp",
                table: "Category");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateTimestamp",
                table: "ScheduleSnapshot",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
