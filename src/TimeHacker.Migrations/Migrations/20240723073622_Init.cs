using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(516)", maxLength: 516, nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(516)", maxLength: 516, nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    MinTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaxTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: false),
                    OptimalTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FixedTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(516)", maxLength: 516, nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    StartTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryDynamicTask",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    DynamicTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDynamicTask", x => new { x.CategoryId, x.DynamicTaskId });
                    table.ForeignKey(
                        name: "FK_CategoryDynamicTask_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryDynamicTask_DynamicTask_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "DynamicTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategoryFixedTask",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    FixedTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryFixedTask", x => new { x.CategoryId, x.FixedTaskId });
                    table.ForeignKey(
                        name: "FK_CategoryFixedTask_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryFixedTask_FixedTask_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FixedTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_UserId",
                table: "Category",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTask_CreatedTimestamp",
                table: "DynamicTask",
                column: "CreatedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTask_IsCompleted",
                table: "DynamicTask",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTask_UserId",
                table: "DynamicTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_CreatedTimestamp",
                table: "FixedTask",
                column: "CreatedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_IsCompleted",
                table: "FixedTask",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_StartTimestamp",
                table: "FixedTask",
                column: "StartTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_UserId",
                table: "FixedTask",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryDynamicTask");

            migrationBuilder.DropTable(
                name: "CategoryFixedTask");

            migrationBuilder.DropTable(
                name: "DynamicTask");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "FixedTask");
        }
    }
}
