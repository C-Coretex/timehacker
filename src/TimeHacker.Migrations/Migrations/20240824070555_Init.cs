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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(516)", maxLength: 516, nullable: true),
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(516)", maxLength: 516, nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    MinTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaxTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: false),
                    OptimalTimeToFinish = table.Column<TimeSpan>(type: "time", nullable: true),
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
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(516)", maxLength: 516, nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    StartTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RepeatingEntity = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ScheduleCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastTaskCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleSnapshot",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    LastUpdateTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleSnapshot", x => new { x.UserId, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "CategoryDynamicTask",
                columns: table => new
                {
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    DynamicTaskId = table.Column<long>(type: "bigint", nullable: false)
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
                        name: "FK_CategoryDynamicTask_DynamicTask_DynamicTaskId",
                        column: x => x.DynamicTaskId,
                        principalTable: "DynamicTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategoryFixedTask",
                columns: table => new
                {
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    FixedTaskId = table.Column<long>(type: "bigint", nullable: false)
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
                        name: "FK_CategoryFixedTask_FixedTask_FixedTaskId",
                        column: x => x.FixedTaskId,
                        principalTable: "FixedTask",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScheduledCategory",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    ParentScheduleEntity = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_ScheduledCategory_ScheduleEntity_ParentScheduleEntity",
                        column: x => x.ParentScheduleEntity,
                        principalTable: "ScheduleEntity",
                        principalColumn: "Id");
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
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentTaskId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledCategoryId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    ParentScheduleEntity = table.Column<long>(type: "bigint", nullable: true),
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
                        name: "FK_ScheduledTask_ScheduleEntity_ParentScheduleEntity",
                        column: x => x.ParentScheduleEntity,
                        principalTable: "ScheduleEntity",
                        principalColumn: "Id");
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
                name: "IX_Category_UserId",
                table: "Category",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDynamicTask_DynamicTaskId",
                table: "CategoryDynamicTask",
                column: "DynamicTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFixedTask_FixedTaskId",
                table: "CategoryFixedTask",
                column: "FixedTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTask_CreatedTimestamp",
                table: "DynamicTask",
                column: "CreatedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTask_UserId",
                table: "DynamicTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_CreatedTimestamp",
                table: "FixedTask",
                column: "CreatedTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_StartTimestamp",
                table: "FixedTask",
                column: "StartTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTask_UserId",
                table: "FixedTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCategory_ParentScheduleEntity",
                table: "ScheduledCategory",
                column: "ParentScheduleEntity");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledCategory_UserId_Date",
                table: "ScheduledCategory",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTask_IsCompleted",
                table: "ScheduledTask",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTask_ParentScheduleEntity",
                table: "ScheduledTask",
                column: "ParentScheduleEntity");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTask_ScheduledCategoryId",
                table: "ScheduledTask",
                column: "ScheduledCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledTask_UserId_Date",
                table: "ScheduledTask",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntity_EndsOn",
                table: "ScheduleEntity",
                column: "EndsOn");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleEntity_UserId",
                table: "ScheduleEntity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleSnapshot_Date",
                table: "ScheduleSnapshot",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleSnapshot_UserId",
                table: "ScheduleSnapshot",
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
                name: "ScheduledTask");

            migrationBuilder.DropTable(
                name: "DynamicTask");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "FixedTask");

            migrationBuilder.DropTable(
                name: "ScheduledCategory");

            migrationBuilder.DropTable(
                name: "ScheduleEntity");

            migrationBuilder.DropTable(
                name: "ScheduleSnapshot");
        }
    }
}
