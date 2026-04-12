using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class FixAdmin2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Users_User_id",
                table: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Admins_User_id",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "User_id",
                table: "Admins");

            migrationBuilder.AddColumn<int>(
                name: "Admin_id",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Admin_id",
                table: "Users",
                column: "Admin_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Admins_Admin_id",
                table: "Users",
                column: "Admin_id",
                principalTable: "Admins",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Admins_Admin_id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Admin_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Admin_id",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "User_id",
                table: "Admins",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_User_id",
                table: "Admins",
                column: "User_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Users_User_id",
                table: "Admins",
                column: "User_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
