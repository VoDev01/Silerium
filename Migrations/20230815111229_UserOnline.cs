using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Silerium.Migrations
{
    /// <inheritdoc />
    public partial class UserOnline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LoggedIn",
                table: "Users",
                newName: "IsOnline");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOnline",
                table: "Users",
                newName: "LoggedIn");
        }
    }
}
