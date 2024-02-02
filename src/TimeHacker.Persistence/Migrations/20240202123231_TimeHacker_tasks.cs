using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TimeHacker_tasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    MinTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaxTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: false),
                    OptimalTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixedTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    StartTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedTasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTasks_Category",
                table: "DynamicTasks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTasks_CreatedTimestamp",
                table: "DynamicTasks",
                column: "CreatedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTasks_IsCompleted",
                table: "DynamicTasks",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTasks_UserId",
                table: "DynamicTasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTasks_Category",
                table: "FixedTasks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTasks_CreatedTimestamp",
                table: "FixedTasks",
                column: "CreatedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTasks_IsCompleted",
                table: "FixedTasks",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTasks_StartTimestamp",
                table: "FixedTasks",
                column: "StartTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTasks_UserId",
                table: "FixedTasks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicTasks");

            migrationBuilder.DropTable(
                name: "FixedTasks");
        }
    }
}
