using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_and_Spa_Wellness.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceServiceCategoryWithForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add CategoryId as nullable first so nothing is lost while the
            // old Category text column still exists alongside it.
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            // Any category name that exists on a Service row but has no
            // matching ServiceCategory yet (typed in via "+ Add new
            // category..." before this migration existed, on any
            // environment - not just the 23 seeded rows) gets a
            // default-icon row created for it here, so no service silently
            // loses its category during the cutover.
            migrationBuilder.Sql(@"
                INSERT INTO ServiceCategories (Name, Icon)
                SELECT DISTINCT s.Category, '🌟'
                FROM Services s
                WHERE NOT EXISTS (SELECT 1 FROM ServiceCategories c WHERE c.Name = s.Category);
            ");

            // Populate CategoryId from the existing Category text before it's dropped.
            migrationBuilder.Sql(@"
                UPDATE Services
                SET CategoryId = (SELECT c.Id FROM ServiceCategories c WHERE c.Name = Services.Category);
            ");

            // SQLite can't ALTER a column to add NOT NULL/a foreign key in
            // place, so the table gets rebuilt under the same name. Dropping
            // Services would normally be blocked by Appointments.ServiceId's
            // foreign key into it, so FK enforcement is turned off around
            // the rebuild (each statement runs outside the migration's
            // transaction so the PRAGMA actually takes effect - SQLite
            // ignores it mid-transaction) and back on immediately after.
            migrationBuilder.Sql("PRAGMA foreign_keys=OFF;", suppressTransaction: true);

            migrationBuilder.Sql(@"
                CREATE TABLE Services_new (
                    Id INTEGER NOT NULL CONSTRAINT PK_Services PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    CategoryId INTEGER NOT NULL,
                    Description TEXT NOT NULL,
                    DurationMinutes INTEGER NOT NULL,
                    Price TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 0,
                    CONSTRAINT FK_Services_ServiceCategories_CategoryId FOREIGN KEY (CategoryId) REFERENCES ServiceCategories (Id) ON DELETE RESTRICT
                );
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
                INSERT INTO Services_new (Id, Name, CategoryId, Description, DurationMinutes, Price, IsActive)
                SELECT Id, Name, CategoryId, Description, DurationMinutes, Price, IsActive FROM Services;
            ", suppressTransaction: true);

            migrationBuilder.Sql("DROP TABLE Services;", suppressTransaction: true);
            migrationBuilder.Sql("ALTER TABLE Services_new RENAME TO Services;", suppressTransaction: true);
            migrationBuilder.Sql("CREATE INDEX IX_Services_CategoryId ON Services (CategoryId);", suppressTransaction: true);

            migrationBuilder.Sql("PRAGMA foreign_keys=ON;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=OFF;", suppressTransaction: true);

            migrationBuilder.Sql(@"
                CREATE TABLE Services_new (
                    Id INTEGER NOT NULL CONSTRAINT PK_Services PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    DurationMinutes INTEGER NOT NULL,
                    Price TEXT NOT NULL,
                    IsActive INTEGER NOT NULL DEFAULT 0
                );
            ", suppressTransaction: true);

            migrationBuilder.Sql(@"
                INSERT INTO Services_new (Id, Name, Category, Description, DurationMinutes, Price, IsActive)
                SELECT s.Id, s.Name, c.Name, s.Description, s.DurationMinutes, s.Price, s.IsActive
                FROM Services s
                JOIN ServiceCategories c ON c.Id = s.CategoryId;
            ", suppressTransaction: true);

            migrationBuilder.Sql("DROP TABLE Services;", suppressTransaction: true);
            migrationBuilder.Sql("ALTER TABLE Services_new RENAME TO Services;", suppressTransaction: true);

            migrationBuilder.Sql("PRAGMA foreign_keys=ON;", suppressTransaction: true);
        }
    }
}
