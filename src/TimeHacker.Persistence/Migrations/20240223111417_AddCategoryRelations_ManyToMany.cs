using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryRelations_ManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FixedTasks_Category",
                table: "FixedTasks");

            migrationBuilder.DropIndex(
                name: "IX_DynamicTasks_Category",
                table: "DynamicTasks");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "FixedTasks");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "DynamicTasks");

            migrationBuilder.CreateTable(
                name: "CategoryDynamicTasks",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    DynamicTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDynamicTasks", x => new { x.CategoryId, x.DynamicTaskId });
                    table.ForeignKey(
                        name: "FK_CategoryDynamicTasks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryDynamicTasks_DynamicTasks_DynamicTaskId",
                        column: x => x.DynamicTaskId,
                        principalTable: "DynamicTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoryFixedTasks",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    FixedTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryFixedTasks", x => new { x.CategoryId, x.FixedTaskId });
                    table.ForeignKey(
                        name: "FK_CategoryFixedTasks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryFixedTasks_FixedTasks_FixedTaskId",
                        column: x => x.FixedTaskId,
                        principalTable: "FixedTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDynamicTasks_DynamicTaskId",
                table: "CategoryDynamicTasks",
                column: "DynamicTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryFixedTasks_FixedTaskId",
                table: "CategoryFixedTasks",
                column: "FixedTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryDynamicTasks");

            migrationBuilder.DropTable(
                name: "CategoryFixedTasks");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "FixedTasks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "DynamicTasks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FixedTasks_Category",
                table: "FixedTasks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicTasks_Category",
                table: "DynamicTasks",
                column: "Category");
        }
    }
}
