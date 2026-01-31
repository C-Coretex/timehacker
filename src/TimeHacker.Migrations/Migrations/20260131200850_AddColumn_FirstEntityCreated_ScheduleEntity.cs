using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddColumn_FirstEntityCreated_ScheduleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "FirstEntityCreated",
                table: "ScheduleEntity",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstEntityCreated",
                table: "ScheduleEntity");
        }
    }
}
