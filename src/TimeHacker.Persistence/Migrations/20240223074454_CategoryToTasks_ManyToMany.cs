using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeHacker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CategoryToTasks_ManyToMany : Migration
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
                });
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
