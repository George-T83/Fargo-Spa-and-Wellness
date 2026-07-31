using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderIdToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "Appointments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ProviderId",
                table: "Appointments",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ProviderId",
                table: "Appointments",
                column: "ProviderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ProviderId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ProviderId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Appointments");
        }
    }
}
