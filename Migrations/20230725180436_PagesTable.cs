using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Silerium.Migrations
{
    /// <inheritdoc />
    public partial class PagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PageId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Pages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_PageId",
                table: "Products",
                column: "PageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Pages_PageId",
                table: "Products",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Pages_PageId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Products_PageId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PageId",
                table: "Products");
        }
    }
}
