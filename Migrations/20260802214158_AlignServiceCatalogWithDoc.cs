using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class AlignServiceCatalogWithDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Targeted pressure on deeper layers of muscle and connective tissue to relieve chronic pain and tightness.", 165m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "A full-body relaxation massage using long, gliding strokes to ease tension and improve circulation. Our most-booked introductory massage.", "Swedish Massage", 120m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "DurationMinutes", "Price" },
                values: new object[] { "Body Treatments", "A detoxifying wrap infused with essential oils to nourish and soften the skin.", 45, 95m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "DurationMinutes", "Price" },
                values: new object[] { "Body Treatments", 60, 110m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Our customized, multi-step facial designed around your specific skin type and concerns.", 135m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 75, 160m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsActive",
                value: false);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Wellness & Add-Ons", "Aromatherapy Enhancement (Add-On)", 15m });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Category", "Description", "DurationMinutes", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 9, "Massage", "Heated basalt stones placed on key points to melt away tension and restore energy flow, paired with a full-body massage.", 75, true, "Hot Stone Therapy", 150m },
                    { 10, "Massage", "Pressure-point therapy on the feet and hands to restore balance and overall wellbeing.", 45, true, "Reflexology", 85m },
                    { 11, "Massage", "A gentle, side-lying massage tailored to the needs of expecting mothers, easing pregnancy-related tension safely.", 60, true, "Prenatal Massage", 130m },
                    { 12, "Massage", "Side-by-side Swedish massages in our private couples suite, complete with champagne.", 90, true, "Couples Retreat", 280m },
                    { 13, "Body Treatments", "Mineral-rich volcanic mud draws out impurities and leaves skin glowing and refreshed.", 60, true, "Volcanic Mud Wrap", 130m },
                    { 14, "Body Treatments", "A full-body exfoliation using mineral salts to remove dead skin and stimulate circulation.", 45, true, "Detox Salt Scrub", 90m },
                    { 15, "Facial & Skincare", "A quick refresh - cleanse, exfoliate, and hydrate for guests short on time.", 30, true, "Express Facial", 70m },
                    { 16, "Facial & Skincare", "A moisture-replenishing treatment for dry or dehydrated skin, leaving it soft and dewy.", 50, true, "Hydrating Facial", 110m },
                    { 17, "Nail Care", "A nourishing manicure with a moisturizing soak, shaping, cuticle care, and polish.", 30, true, "Hydrating Manicure", 55m },
                    { 18, "Nail Care", "A long-lasting, chip-resistant gel polish manicure with full nail prep.", 45, true, "Gel Manicure", 65m },
                    { 19, "Nail Care", "A relaxing foot soak, exfoliation, nail shaping, and polish.", 45, true, "Classic Pedicure", 60m },
                    { 20, "Nail Care", "An extended pedicure with a warm paraffin treatment and calf massage.", 60, true, "Deluxe Spa Pedicure", 85m },
                    { 21, "Wellness & Add-Ons", "Private access to our dry sauna to relax muscles and promote detoxification.", 30, true, "Sauna Session", 25m },
                    { 22, "Wellness & Add-Ons", "Private access to our steam room to open pores and ease respiratory tension.", 30, true, "Steam Room Access", 20m },
                    { 23, "Wellness & Add-Ons", "Add a soothing scalp and head massage to any service.", 15, true, "Scalp & Head Massage (Add-On)", 20m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Targeted pressure to release deep muscle tension.", 145m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "A gentle, flowing massage to ease everyday stress.", "Swedish Relaxation Massage", 110m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Category", "Description", "DurationMinutes", "Price" },
                values: new object[] { "Body", "Nourishing botanical wrap with essential oils.", 75, 120m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Category", "DurationMinutes", "Price" },
                values: new object[] { "Body", 50, 100m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Price" },
                values: new object[] { "Customized facial for radiant, glowing skin.", 95m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 130m });

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsActive",
                value: true);

            migrationBuilder.UpdateData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Category", "Name", "Price" },
                values: new object[] { "Wellness", "Aromatherapy Enhancement", 25m });
        }
    }
}
