using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class FixAdmin3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<long>(
                name: "TelegramId",
                table: "Admins",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TelegramId",
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
    }
}
