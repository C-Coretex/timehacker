using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Add_ScheduledTasksAndCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FixedTask_IsCompleted",
                table: "FixedTask");

            migrationBuilder.DropIndex(
                name: "IX_DynamicTask_IsCompleted",
                table: "DynamicTask");

            migrationBuilder.DropColumn(
                name: "ScheduleData",
                table: "ScheduleSnapshot");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "FixedTask");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "DynamicTask");

            migrationBuilder.CreateTable(
                name: "ScheduledCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    UpdatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledCategory_ScheduleSnapshot_UserId_Date",
                        columns: x => new { x.UserId, x.Date },
                        principalTable: "ScheduleSnapshot",
                        principalColumns: new[] { "UserId", "Date" });
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentTaskId = table.Column<int>(type: "int", nullable: false),
                    ScheduledCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    Start = table.Column<TimeSpan>(type: "time", nullable: false),
                    End = table.Column<TimeSpan>(type: "time", nullable: false),
                    UpdatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledTask_ScheduleSnapshot_UserId_Date",
                        columns: x => new { x.UserId, x.Date },
                        principalTable: "ScheduleSnapshot",
                        principalColumns: new[] { "UserId", "Date" });
                    table.ForeignKey(
                        name: "FK_ScheduledTask_ScheduledCategory_ScheduledCategoryId",
                        column: x => x.ScheduledCategoryId,
                        principalTable: "ScheduledCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleSnapshot_Date",
                table: "ScheduleSnapshot",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCategory_UserId_Date",
                table: "ScheduledCategory",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTask_IsCompleted",
                table: "ScheduledTask",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTask_ScheduledCategoryId",
                table: "ScheduledTask",
                column: "ScheduledCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTask_UserId_Date",
                table: "ScheduledTask",
                columns: new[] { "UserId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledTask");

            migrationBuilder.DropTable(
                name: "ScheduledCategory");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleSnapshot_Date",
                table: "ScheduleSnapshot");

            migrationBuilder.AddColumn<byte[]>(
                name: "ScheduleData",
                table: "ScheduleSnapshot",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "FixedTask",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "DynamicTask",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_IsCompleted",
                table: "FixedTask",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTask_IsCompleted",
                table: "DynamicTask",
                column: "IsCompleted");
        }
    }
}
