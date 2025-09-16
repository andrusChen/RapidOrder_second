using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RapidOrder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionMaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceCode = table.Column<string>(type: "TEXT", nullable: false),
                    ButtonNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    MissionType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlaceGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    PlaceGroupId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_PlaceGroups_PlaceGroupId",
                        column: x => x.PlaceGroupId,
                        principalTable: "PlaceGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Watches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Serial = table.Column<string>(type: "TEXT", nullable: false),
                    AssignedUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    BatteryPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Watches_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CallButtons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceCode = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ButtonId = table.Column<string>(type: "TEXT", nullable: false),
                    PlaceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallButtons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CallButtons_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AcknowledgedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AssignedUserId = table.Column<long>(type: "INTEGER", nullable: true),
                    PlaceId = table.Column<int>(type: "INTEGER", nullable: true),
                    SourceDecoded = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    SourceButton = table.Column<int>(type: "INTEGER", nullable: true),
                    MissionDurationSeconds = table.Column<long>(type: "INTEGER", nullable: true),
                    IdleTimeSeconds = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Missions_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Missions_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MissionId = table.Column<long>(type: "INTEGER", nullable: true),
                    PlaceId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: true),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventLogs_Missions_MissionId",
                        column: x => x.MissionId,
                        principalTable: "Missions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventLogs_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ActionMaps_DeviceCode_ButtonNumber",
                table: "ActionMaps",
                columns: new[] { "DeviceCode", "ButtonNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CallButtons_DeviceCode",
                table: "CallButtons",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CallButtons_PlaceId",
                table: "CallButtons",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_MissionId",
                table: "EventLogs",
                column: "MissionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_PlaceId",
                table: "EventLogs",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_UserId",
                table: "EventLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_AssignedUserId",
                table: "Missions",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Missions_PlaceId_StartedAt",
                table: "Missions",
                columns: new[] { "PlaceId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Places_Number",
                table: "Places",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_Places_PlaceGroupId",
                table: "Places",
                column: "PlaceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Watches_AssignedUserId",
                table: "Watches",
                column: "AssignedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionMaps");

            migrationBuilder.DropTable(
                name: "CallButtons");

            migrationBuilder.DropTable(
                name: "EventLogs");

            migrationBuilder.DropTable(
                name: "Watches");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "PlaceGroups");
        }
    }
}
