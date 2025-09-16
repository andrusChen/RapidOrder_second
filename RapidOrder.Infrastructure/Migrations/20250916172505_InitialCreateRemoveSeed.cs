using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RapidOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateRemoveSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CallButtons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CallButtons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Places",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Places",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Places",
                columns: new[] { "Id", "Description", "Number", "PlaceGroupId" },
                values: new object[,]
                {
                    { 1, "Table 1", 101, null },
                    { 2, "Table 2", 102, null }
                });

            migrationBuilder.InsertData(
                table: "CallButtons",
                columns: new[] { "Id", "ButtonId", "DeviceCode", "Label", "PlaceId" },
                values: new object[,]
                {
                    { 1, "ACEF", "ACEF", "Table 1 Button", 1 },
                    { 2, "4D3E", "4D3F", "Table 2 Button", 2 }
                });
        }
    }
}
