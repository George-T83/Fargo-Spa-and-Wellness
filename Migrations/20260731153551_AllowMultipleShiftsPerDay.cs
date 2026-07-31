using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleShiftsPerDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderShifts_ProviderId_DayOfWeek",
                table: "ProviderShifts");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderShifts_ProviderId_DayOfWeek",
                table: "ProviderShifts",
                columns: new[] { "ProviderId", "DayOfWeek" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProviderShifts_ProviderId_DayOfWeek",
                table: "ProviderShifts");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderShifts_ProviderId_DayOfWeek",
                table: "ProviderShifts",
                columns: new[] { "ProviderId", "DayOfWeek" },
                unique: true);
        }
    }
}
