using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeWindowToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Category",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Category",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Category");
        }
    }
}
