using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RapidOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceAssignedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedUserId",
                table: "Places",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Places_AssignedUserId",
                table: "Places",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Places_Users_AssignedUserId",
                table: "Places",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Places_Users_AssignedUserId",
                table: "Places");

            migrationBuilder.DropIndex(
                name: "IX_Places_AssignedUserId",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "Places");
        }
    }
}
