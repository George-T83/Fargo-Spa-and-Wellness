using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    IsOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    OpenTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessHoursOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsClosed = table.Column<bool>(type: "INTEGER", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHoursOverrides", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BusinessHours",
                columns: new[] { "Id", "CloseTime", "DayOfWeek", "IsOpen", "OpenTime" },
                values: new object[,]
                {
                    { 1, new TimeSpan(0, 17, 0, 0, 0), 0, true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 2, new TimeSpan(0, 17, 0, 0, 0), 1, true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 3, new TimeSpan(0, 17, 0, 0, 0), 2, true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 4, new TimeSpan(0, 17, 0, 0, 0), 3, true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 5, new TimeSpan(0, 17, 0, 0, 0), 4, true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 6, new TimeSpan(0, 17, 0, 0, 0), 5, true, new TimeSpan(0, 9, 0, 0, 0) },
                    { 7, new TimeSpan(0, 17, 0, 0, 0), 6, true, new TimeSpan(0, 9, 0, 0, 0) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHours_DayOfWeek",
                table: "BusinessHours",
                column: "DayOfWeek",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHoursOverrides_Date",
                table: "BusinessHoursOverrides",
                column: "Date",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessHours");

            migrationBuilder.DropTable(
                name: "BusinessHoursOverrides");
        }
    }
}
