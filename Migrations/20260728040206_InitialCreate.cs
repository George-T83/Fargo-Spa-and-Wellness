using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Category", "Description", "DurationMinutes", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Massage", "Targeted pressure to release deep muscle tension.", 90, "Deep Tissue Massage", 145m },
                    { 2, "Massage", "A gentle, flowing massage to ease everyday stress.", 60, "Swedish Relaxation Massage", 110m },
                    { 3, "Body", "Nourishing botanical wrap with essential oils.", 75, "Aromatherapy Body Wrap", 120m },
                    { 4, "Body", "A gentle, all-over exfoliating polish that leaves skin smooth, soft, and radiant.", 50, "Body Polish & Buff", 100m },
                    { 5, "Facial & Skincare", "Customized facial for radiant, glowing skin.", 60, "Signature Facial", 95m },
                    { 6, "Facial & Skincare", "A collagen-boosting treatment targeting fine lines and loss of elasticity.", 60, "Anti-Aging Collagen Facial", 130m },
                    { 7, "Wellness", "A full-body relaxation journey with lavender.", 120, "Lavender Relaxation Ritual", 180m },
                    { 8, "Wellness", "Add a custom essential oil blend to any massage or body treatment.", 15, "Aromatherapy Enhancement", 25m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services");
        }
    }
}
