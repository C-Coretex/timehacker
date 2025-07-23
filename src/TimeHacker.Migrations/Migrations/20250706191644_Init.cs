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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    IdentityId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PhoneNumberForNotifications = table.Column<string>(type: "text", nullable: true),
                    EmailForNotifications = table.Column<string>(type: "text", nullable: true),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(516)", maxLength: 516, nullable: true),
                    Priority = table.Column<byte>(type: "smallint", nullable: false),
                    MinTimeToFinish = table.Column<TimeSpan>(type: "interval", nullable: false),
                    MaxTimeToFinish = table.Column<TimeSpan>(type: "interval", nullable: false),
                    OptimalTimeToFinish = table.Column<TimeSpan>(type: "interval", nullable: true),
                    CreatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicTask_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepeatingEntity = table.Column<byte[]>(type: "bytea", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastEntityCreated = table.Column<DateOnly>(type: "date", nullable: true),
                    EndsOn = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleEntity_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleSnapshot",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    LastUpdateTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleSnapshot", x => new { x.UserId, x.Date });
                    table.ForeignKey(
                        name: "FK_ScheduleSnapshot_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Color = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tag_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    ScheduleEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(516)", maxLength: 516, nullable: true),
                    Color = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_ScheduleEntity_ScheduleEntityId",
                        column: x => x.ScheduleEntityId,
                        principalTable: "ScheduleEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Category_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FixedTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", maxLength: 450, nullable: false),
                    ScheduleEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "character varying(516)", maxLength: 516, nullable: true),
                    Priority = table.Column<byte>(type: "smallint", nullable: false),
                    StartTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FixedTask_ScheduleEntity_ScheduleEntityId",
                        column: x => x.ScheduleEntityId,
                        principalTable: "ScheduleEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FixedTask_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentScheduleEntity = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<TimeSpan>(type: "interval", nullable: false),
                    End = table.Column<TimeSpan>(type: "interval", nullable: false),
                    UpdatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_ScheduledCategory_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagDynamicTask",
                columns: table => new
                {
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagDynamicTask", x => new { x.TagId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_TagDynamicTask_DynamicTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "DynamicTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagDynamicTask_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CategoryDynamicTask",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    DynamicTaskId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    FixedTaskId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "TagFixedTask",
                columns: table => new
                {
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagFixedTask", x => new { x.TagId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_TagFixedTask_FixedTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "FixedTask",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagFixedTask_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ScheduledTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentScheduleEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsFixed = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<byte>(type: "smallint", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Start = table.Column<TimeSpan>(type: "interval", nullable: false),
                    End = table.Column<TimeSpan>(type: "interval", nullable: false),
                    UpdatedTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledTask_ScheduleEntity_ParentScheduleEntityId",
                        column: x => x.ParentScheduleEntityId,
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
                    table.ForeignKey(
                        name: "FK_ScheduledTask_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_ScheduleEntityId",
                table: "Category",
                column: "ScheduleEntityId",
                unique: true);

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
                name: "IX_FixedTask_ScheduleEntityId",
                table: "FixedTask",
                column: "ScheduleEntityId",
                unique: true);

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
                name: "IX_ScheduledTask_ParentScheduleEntityId",
                table: "ScheduledTask",
                column: "ParentScheduleEntityId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Tag_Category",
                table: "Tag",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_UserId",
                table: "Tag",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TagDynamicTask_TaskId",
                table: "TagDynamicTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TagFixedTask_TaskId",
                table: "TagFixedTask",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_User_IdentityId",
                table: "User",
                column: "IdentityId",
                unique: true);
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
                name: "TagDynamicTask");

            migrationBuilder.DropTable(
                name: "TagFixedTask");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "ScheduledCategory");

            migrationBuilder.DropTable(
                name: "DynamicTask");

            migrationBuilder.DropTable(
                name: "FixedTask");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "ScheduleSnapshot");

            migrationBuilder.DropTable(
                name: "ScheduleEntity");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
