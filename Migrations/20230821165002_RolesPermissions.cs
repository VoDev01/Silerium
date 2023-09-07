using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Silerium.Migrations
{
    /// <inheritdoc />
    public partial class RolesPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    Granted = table.Column<bool>(type: "bit", nullable: false),
                    GrantedByUser = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "PermissionName" },
                values: new object[,]
                {
                    { 1, "Permission.Product.View" },
                    { 2, "Permission.Product.Create" },
                    { 3, "Permission.Product.Edit" },
                    { 4, "Permission.Product.Delete" },
                    { 5, "Permission.Product.DownloadData" },
                    { 6, "Permission.Category.View" },
                    { 7, "Permission.Category.Create" },
                    { 8, "Permission.Category.Edit" },
                    { 9, "Permission.Category.Delete" },
                    { 10, "Permission.Category.DownloadData" },
                    { 11, "Permission.Subcategory.View" },
                    { 12, "Permission.Subcategory.Create" },
                    { 13, "Permission.Subcategory.Edit" },
                    { 14, "Permission.Subcategory.Delete" },
                    { 15, "Permission.Subcategory.DownloadData" },
                    { 16, "Permission.User.View" },
                    { 17, "Permission.User.Create" },
                    { 18, "Permission.User.Edit" },
                    { 19, "Permission.User.Delete" },
                    { 20, "Permission.User.DownloadData" },
                    { 21, "Permission.Role.View" },
                    { 22, "Permission.Role.Create" },
                    { 23, "Permission.Role.Edit" },
                    { 24, "Permission.Role.Delete" },
                    { 25, "Permission.Role.DownloadData" },
                    { 26, "Permission.Permission.View" },
                    { 27, "Permission.Permission.Create" },
                    { 28, "Permission.Permission.Edit" },
                    { 29, "Permission.Permission.Delete" },
                    { 30, "Permission.Permission.DownloadData" },
                    { 31, "Permission.Order.View" },
                    { 32, "Permission.Order.Create" },
                    { 33, "Permission.Order.Edit" },
                    { 34, "Permission.Order.Delete" },
                    { 35, "Permission.Order.DownloadData" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "SuperAdmin" },
                    { 2, "Admin" },
                    { 3, "Moderator" },
                    { 4, "Manager" },
                    { 5, "User" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId", "Granted", "GrantedByUser" },
                values: new object[,]
                {
                    { 1, 1, true, "Dev" },
                    { 2, 1, true, "Dev" },
                    { 3, 1, true, "Dev" },
                    { 4, 1, true, "Dev" },
                    { 5, 1, true, "Dev" },
                    { 6, 1, true, "Dev" },
                    { 7, 1, true, "Dev" },
                    { 8, 1, true, "Dev" },
                    { 9, 1, true, "Dev" },
                    { 10, 1, true, "Dev" },
                    { 11, 1, true, "Dev" },
                    { 12, 1, true, "Dev" },
                    { 13, 1, true, "Dev" },
                    { 14, 1, true, "Dev" },
                    { 15, 1, true, "Dev" },
                    { 16, 1, true, "Dev" },
                    { 17, 1, true, "Dev" },
                    { 18, 1, true, "Dev" },
                    { 19, 1, true, "Dev" },
                    { 20, 1, true, "Dev" },
                    { 21, 1, true, "Dev" },
                    { 22, 1, true, "Dev" },
                    { 23, 1, true, "Dev" },
                    { 24, 1, true, "Dev" },
                    { 25, 1, true, "Dev" },
                    { 26, 1, true, "Dev" },
                    { 27, 1, true, "Dev" },
                    { 28, 1, true, "Dev" },
                    { 29, 1, true, "Dev" },
                    { 30, 1, true, "Dev" },
                    { 31, 1, true, "Dev" },
                    { 32, 1, true, "Dev" },
                    { 33, 1, true, "Dev" },
                    { 34, 1, true, "Dev" },
                    { 35, 1, true, "Dev" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                table: "RoleUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }
    }
}
