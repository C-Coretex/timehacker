using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_ScheduleEntity_ScheduleEntityId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryDynamicTask_Category_CategoryId",
                table: "CategoryDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryDynamicTask_DynamicTask_DynamicTaskId",
                table: "CategoryDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFixedTask_Category_CategoryId",
                table: "CategoryFixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFixedTask_FixedTask_FixedTaskId",
                table: "CategoryFixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedTask_ScheduleEntity_ScheduleEntityId",
                table: "FixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledCategory_ScheduleEntity_ParentScheduleEntity",
                table: "ScheduledCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledCategory_ScheduleSnapshot_UserId_Date",
                table: "ScheduledCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledTask_ScheduleEntity_ParentScheduleEntityId",
                table: "ScheduledTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledTask_ScheduleSnapshot_UserId_Date",
                table: "ScheduledTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledTask_ScheduledCategory_ScheduledCategoryId",
                table: "ScheduledTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagDynamicTask_DynamicTask_TaskId",
                table: "TagDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagDynamicTask_Tag_TagId",
                table: "TagDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFixedTask_FixedTask_TaskId",
                table: "TagFixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFixedTask_Tag_TagId",
                table: "TagFixedTask");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_ScheduleEntity_ScheduleEntityId",
                table: "Category",
                column: "ScheduleEntityId",
                principalTable: "ScheduleEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryDynamicTask_Category_CategoryId",
                table: "CategoryDynamicTask",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryDynamicTask_DynamicTask_DynamicTaskId",
                table: "CategoryDynamicTask",
                column: "DynamicTaskId",
                principalTable: "DynamicTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFixedTask_Category_CategoryId",
                table: "CategoryFixedTask",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFixedTask_FixedTask_FixedTaskId",
                table: "CategoryFixedTask",
                column: "FixedTaskId",
                principalTable: "FixedTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FixedTask_ScheduleEntity_ScheduleEntityId",
                table: "FixedTask",
                column: "ScheduleEntityId",
                principalTable: "ScheduleEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledCategory_ScheduleEntity_ParentScheduleEntity",
                table: "ScheduledCategory",
                column: "ParentScheduleEntity",
                principalTable: "ScheduleEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledCategory_ScheduleSnapshot_UserId_Date",
                table: "ScheduledCategory",
                columns: new[] { "UserId", "Date" },
                principalTable: "ScheduleSnapshot",
                principalColumns: new[] { "UserId", "Date" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledTask_ScheduleEntity_ParentScheduleEntityId",
                table: "ScheduledTask",
                column: "ParentScheduleEntityId",
                principalTable: "ScheduleEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledTask_ScheduleSnapshot_UserId_Date",
                table: "ScheduledTask",
                columns: new[] { "UserId", "Date" },
                principalTable: "ScheduleSnapshot",
                principalColumns: new[] { "UserId", "Date" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledTask_ScheduledCategory_ScheduledCategoryId",
                table: "ScheduledTask",
                column: "ScheduledCategoryId",
                principalTable: "ScheduledCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagDynamicTask_DynamicTask_TaskId",
                table: "TagDynamicTask",
                column: "TaskId",
                principalTable: "DynamicTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagDynamicTask_Tag_TagId",
                table: "TagDynamicTask",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagFixedTask_FixedTask_TaskId",
                table: "TagFixedTask",
                column: "TaskId",
                principalTable: "FixedTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagFixedTask_Tag_TagId",
                table: "TagFixedTask",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_ScheduleEntity_ScheduleEntityId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryDynamicTask_Category_CategoryId",
                table: "CategoryDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryDynamicTask_DynamicTask_DynamicTaskId",
                table: "CategoryDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFixedTask_Category_CategoryId",
                table: "CategoryFixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryFixedTask_FixedTask_FixedTaskId",
                table: "CategoryFixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_FixedTask_ScheduleEntity_ScheduleEntityId",
                table: "FixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledCategory_ScheduleEntity_ParentScheduleEntity",
                table: "ScheduledCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledCategory_ScheduleSnapshot_UserId_Date",
                table: "ScheduledCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledTask_ScheduleEntity_ParentScheduleEntityId",
                table: "ScheduledTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledTask_ScheduleSnapshot_UserId_Date",
                table: "ScheduledTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduledTask_ScheduledCategory_ScheduledCategoryId",
                table: "ScheduledTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagDynamicTask_DynamicTask_TaskId",
                table: "TagDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagDynamicTask_Tag_TagId",
                table: "TagDynamicTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFixedTask_FixedTask_TaskId",
                table: "TagFixedTask");

            migrationBuilder.DropForeignKey(
                name: "FK_TagFixedTask_Tag_TagId",
                table: "TagFixedTask");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_ScheduleEntity_ScheduleEntityId",
                table: "Category",
                column: "ScheduleEntityId",
                principalTable: "ScheduleEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryDynamicTask_Category_CategoryId",
                table: "CategoryDynamicTask",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryDynamicTask_DynamicTask_DynamicTaskId",
                table: "CategoryDynamicTask",
                column: "DynamicTaskId",
                principalTable: "DynamicTask",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFixedTask_Category_CategoryId",
                table: "CategoryFixedTask",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryFixedTask_FixedTask_FixedTaskId",
                table: "CategoryFixedTask",
                column: "FixedTaskId",
                principalTable: "FixedTask",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FixedTask_ScheduleEntity_ScheduleEntityId",
                table: "FixedTask",
                column: "ScheduleEntityId",
                principalTable: "ScheduleEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledCategory_ScheduleEntity_ParentScheduleEntity",
                table: "ScheduledCategory",
                column: "ParentScheduleEntity",
                principalTable: "ScheduleEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledCategory_ScheduleSnapshot_UserId_Date",
                table: "ScheduledCategory",
                columns: new[] { "UserId", "Date" },
                principalTable: "ScheduleSnapshot",
                principalColumns: new[] { "UserId", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledTask_ScheduleEntity_ParentScheduleEntityId",
                table: "ScheduledTask",
                column: "ParentScheduleEntityId",
                principalTable: "ScheduleEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledTask_ScheduleSnapshot_UserId_Date",
                table: "ScheduledTask",
                columns: new[] { "UserId", "Date" },
                principalTable: "ScheduleSnapshot",
                principalColumns: new[] { "UserId", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduledTask_ScheduledCategory_ScheduledCategoryId",
                table: "ScheduledTask",
                column: "ScheduledCategoryId",
                principalTable: "ScheduledCategory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagDynamicTask_DynamicTask_TaskId",
                table: "TagDynamicTask",
                column: "TaskId",
                principalTable: "DynamicTask",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagDynamicTask_Tag_TagId",
                table: "TagDynamicTask",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagFixedTask_FixedTask_TaskId",
                table: "TagFixedTask",
                column: "TaskId",
                principalTable: "FixedTask",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagFixedTask_Tag_TagId",
                table: "TagFixedTask",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id");
        }
    }
}
