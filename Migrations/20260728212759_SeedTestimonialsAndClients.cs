using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestimonialsAndClients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sarah.mitchell@example.com", "Sarah", "Mitchell", "seed-no-login", "555-0201", "Client" },
                    { 2, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "james.whitfield@example.com", "James", "Whitfield", "seed-no-login", "555-0202", "Client" },
                    { 3, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "priya.anand@example.com", "Priya", "Anand", "seed-no-login", "555-0203", "Client" }
                });

            migrationBuilder.InsertData(
                table: "Testimonials",
                columns: new[] { "Id", "ApprovalStatus", "ClientId", "CreatedAt", "Rating", "ReviewText" },
                values: new object[,]
                {
                    { 1, "Approved", 1, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "This was exactly what I needed after months of desk work. My shoulders finally feel loose again." },
                    { 2, "Approved", 2, new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "My skin has never looked better. The esthetician really listened to what I wanted." },
                    { 3, "Approved", 3, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Such a relaxing experience from start to finish. I'll definitely be back." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Testimonials",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Testimonials",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Testimonials",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
