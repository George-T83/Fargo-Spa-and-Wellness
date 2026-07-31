using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class SeedServiceNoteReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "Role" },
                values: new object[,]
                {
                    { 100, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "lena.fischer@example.com", "Lena", "Fischer", "seed-no-login", "555-0301", "Client" },
                    { 101, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "robert.chen@example.com", "Robert", "Chen", "seed-no-login", "555-0302", "Client" },
                    { 102, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "megan.delgado@example.com", "Megan", "Delgado", "seed-no-login", "555-0303", "Client" },
                    { 103, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "tobias.brandt@example.com", "Tobias", "Brandt", "seed-no-login", "555-0304", "Client" },
                    { 104, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "yuki.tanaka@example.com", "Yuki", "Tanaka", "seed-no-login", "555-0305", "Client" },
                    { 105, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "devon.cole@example.com", "Devon", "Cole", "seed-no-login", "555-0401", "Provider" },
                    { 106, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "priya.raman@example.com", "Priya", "Raman", "seed-no-login", "555-0402", "Provider" },
                    { 107, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "marcus.whitfield@example.com", "Marcus", "Whitfield", "seed-no-login", "555-0403", "Provider" },
                    { 108, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "sofia.lindqvist@example.com", "Sofia", "Lindqvist", "seed-no-login", "555-0404", "Provider" }
                });

            migrationBuilder.InsertData(
                table: "ServiceNotes",
                columns: new[] { "Id", "AuthorId", "ClientId", "CreatedAt", "NoteText", "NoteType" },
                values: new object[,]
                {
                    { 1, 105, 100, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Severe allergy to lavender oil — avoid all lavender-based products.", "Allergy" },
                    { 2, 105, 100, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Prefers firm pressure and warm room temperature.", "Preference" },
                    { 3, 106, 101, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mild sensitivity to fragrances; use unscented products.", "Allergy" },
                    { 4, 107, 102, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Recovering from shoulder injury — avoid deep pressure on right shoulder.", "General" },
                    { 5, 106, 103, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Likes the calming playlist and dim lighting.", "Preference" },
                    { 6, 108, 104, new DateTime(2026, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Allergic to nuts — check all product ingredient lists.", "Allergy" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceNotes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceNotes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceNotes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServiceNotes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ServiceNotes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServiceNotes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 108);
        }
    }
}
