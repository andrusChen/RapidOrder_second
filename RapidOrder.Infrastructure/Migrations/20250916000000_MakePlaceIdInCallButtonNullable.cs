using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RapidOrder.Infrastructure.Migrations
{
    public partial class MakePlaceIdInCallButtonNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CallButtons_Places_PlaceId",
                table: "CallButtons");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "CallButtons",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_CallButtons_Places_PlaceId",
                table: "CallButtons",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CallButtons_Places_PlaceId",
                table: "CallButtons");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "CallButtons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CallButtons_Places_PlaceId",
                table: "CallButtons",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
