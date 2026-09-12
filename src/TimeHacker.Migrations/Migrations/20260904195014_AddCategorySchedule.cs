using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddCategorySchedule : Migration
    {
        private static readonly string[] UserIdDateIndexColumns = ["UserId", "Date"];

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_ScheduleEntity_ScheduleEntityId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_ScheduleEntityId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ScheduleEntityId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Category");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryScheduleId",
                table: "ScheduledCategory",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ScheduleDescription",
                table: "ScheduledCategory",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategorySchedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(516)", maxLength: 516, nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategorySchedule_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategorySchedule_ScheduleEntity_ScheduleEntityId",
                        column: x => x.ScheduleEntityId,
                        principalTable: "ScheduleEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategorySchedule_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategorySchedule_CategoryId",
                table: "CategorySchedule",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySchedule_ScheduleEntityId",
                table: "CategorySchedule",
                column: "ScheduleEntityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategorySchedule_UserId",
                table: "CategorySchedule",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySchedule_UserId_Date",
                table: "CategorySchedule",
                columns: UserIdDateIndexColumns);

            migrationBuilder.Sql("ALTER TABLE \"CategorySchedule\" ENABLE ROW LEVEL SECURITY;\r\nDROP POLICY IF EXISTS \"CategorySchedule_rls_policy\" ON \"CategorySchedule\";\r\nCREATE POLICY \"CategorySchedule_rls_policy\" ON \"CategorySchedule\"\r\n    FOR ALL\r\n    TO application_user\r\n    USING (\"UserId\" = current_setting('app.user_id', true)::uuid);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategorySchedule");

            migrationBuilder.DropColumn(
                name: "ParentCategoryScheduleId",
                table: "ScheduledCategory");

            migrationBuilder.DropColumn(
                name: "ScheduleDescription",
                table: "ScheduledCategory");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Category",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Category",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<Guid>(
                name: "ScheduleEntityId",
                table: "Category",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Category",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Category_ScheduleEntityId",
                table: "Category",
                column: "ScheduleEntityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Category_ScheduleEntity_ScheduleEntityId",
                table: "Category",
                column: "ScheduleEntityId",
                principalTable: "ScheduleEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
