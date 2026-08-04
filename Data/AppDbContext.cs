using Family_and_Spa_Wellness.Models;
using Microsoft.EntityFrameworkCore;

namespace Family_and_Spa_Wellness.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Service> Services => Set<Service>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<ServiceNote> ServiceNotes => Set<ServiceNote>();
    public DbSet<ProviderAvailability> ProviderAvailability => Set<ProviderAvailability>();
    public DbSet<ProviderShift> ProviderShifts => Set<ProviderShift>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<BusinessHours> BusinessHours => Set<BusinessHours>();
    public DbSet<BusinessHoursOverride> BusinessHoursOverrides => Set<BusinessHoursOverride>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Client)
            .WithMany()
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Service)
            .WithMany()
            .HasForeignKey(a => a.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Provider)
            .WithMany()
            .HasForeignKey(a => a.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceNote>()
            .HasOne(n => n.Client)
            .WithMany()
            .HasForeignKey(n => n.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceNote>()
            .HasOne(n => n.Author)
            .WithMany()
            .HasForeignKey(n => n.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProviderAvailability>()
            .HasIndex(pa => new { pa.ProviderId, pa.Date })
            .IsUnique();

        modelBuilder.Entity<ProviderAvailability>()
            .HasOne(pa => pa.Provider)
            .WithMany()
            .HasForeignKey(pa => pa.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProviderShift>()
            .HasIndex(ps => new { ps.ProviderId, ps.DayOfWeek });

        modelBuilder.Entity<ProviderShift>()
            .HasOne(ps => ps.Provider)
            .WithMany()
            .HasForeignKey(ps => ps.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceCategory>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Service>()
            .HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BusinessHours>()
            .HasIndex(h => h.DayOfWeek)
            .IsUnique();

        modelBuilder.Entity<BusinessHoursOverride>()
            .HasIndex(o => o.Date)
            .IsUnique();

        // Seeded to a single consistent 9 AM - 5 PM, seven days a week -
        // replaces the three different hardcoded values that used to live
        // independently in Book.razor, Reschedule.razor, and Contact.razor.
        // Admins can edit per day (including marking a day closed) from
        // /admin/hours.
        modelBuilder.Entity<Models.BusinessHours>().HasData(
            new Models.BusinessHours { Id = 1, DayOfWeek = DayOfWeek.Sunday, IsOpen = true, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0) },
            new Models.BusinessHours { Id = 2, DayOfWeek = DayOfWeek.Monday, IsOpen = true, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0) },
            new Models.BusinessHours { Id = 3, DayOfWeek = DayOfWeek.Tuesday, IsOpen = true, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0) },
            new Models.BusinessHours { Id = 4, DayOfWeek = DayOfWeek.Wednesday, IsOpen = true, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0) },
            new Models.BusinessHours { Id = 5, DayOfWeek = DayOfWeek.Thursday, IsOpen = true, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0) },
            new Models.BusinessHours { Id = 6, DayOfWeek = DayOfWeek.Friday, IsOpen = true, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0) },
            new Models.BusinessHours { Id = 7, DayOfWeek = DayOfWeek.Saturday, IsOpen = true, OpenTime = new TimeSpan(9, 0, 0), CloseTime = new TimeSpan(17, 0, 0) }
        );

        // Seeded with the app's original hardcoded icon set, plus "Wellness"
        // for the deactivated Lavender Relaxation Ritual service, which
        // predates the current category naming.
        modelBuilder.Entity<ServiceCategory>().HasData(
            new ServiceCategory { Id = 1, Name = "Massage", Icon = "\U0001F486" },
            new ServiceCategory { Id = 2, Name = "Facial & Skincare", Icon = "✨" },
            new ServiceCategory { Id = 3, Name = "Body Treatments", Icon = "\U0001F9D6" },
            new ServiceCategory { Id = 4, Name = "Nail Care", Icon = "\U0001F485" },
            new ServiceCategory { Id = 5, Name = "Wellness & Add-Ons", Icon = "\U0001F33F" },
            new ServiceCategory { Id = 6, Name = "Wellness", Icon = "\U0001F31F" }
        );

        // Canonical catalog per the Requirements and Design doc's Appendix
        // ("Service Catalog, Pricing & Business Policies" - the single source
        // of truth for FSW-7/FSW-8/FSW-22). Id 7 (Lavender Relaxation Ritual)
        // predates that doc and isn't in it, so it's deactivated rather than
        // deleted to avoid breaking any appointment history referencing it.
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Deep Tissue Massage", CategoryId = 1, Description = "Targeted pressure on deeper layers of muscle and connective tissue to relieve chronic pain and tightness.", DurationMinutes = 90, Price = 165m, IsActive = true },
            new Service { Id = 2, Name = "Swedish Massage", CategoryId = 1, Description = "A full-body relaxation massage using long, gliding strokes to ease tension and improve circulation. Our most-booked introductory massage.", DurationMinutes = 60, Price = 120m, IsActive = true },
            new Service { Id = 3, Name = "Aromatherapy Body Wrap", CategoryId = 3, Description = "A detoxifying wrap infused with essential oils to nourish and soften the skin.", DurationMinutes = 45, Price = 95m, IsActive = true },
            new Service { Id = 4, Name = "Body Polish & Buff", CategoryId = 3, Description = "A gentle, all-over exfoliating polish that leaves skin smooth, soft, and radiant.", DurationMinutes = 60, Price = 110m, IsActive = true },
            new Service { Id = 5, Name = "Signature Facial", CategoryId = 2, Description = "Our customized, multi-step facial designed around your specific skin type and concerns.", DurationMinutes = 60, Price = 135m, IsActive = true },
            new Service { Id = 6, Name = "Anti-Aging Collagen Facial", CategoryId = 2, Description = "A collagen-boosting treatment targeting fine lines and loss of elasticity.", DurationMinutes = 75, Price = 160m, IsActive = true },
            new Service { Id = 7, Name = "Lavender Relaxation Ritual", CategoryId = 6, Description = "A full-body relaxation journey with lavender.", DurationMinutes = 120, Price = 180m, IsActive = false },
            new Service { Id = 8, Name = "Aromatherapy Enhancement (Add-On)", CategoryId = 5, Description = "Add a custom essential oil blend to any massage or body treatment.", DurationMinutes = 15, Price = 15m, IsActive = true },
            new Service { Id = 9, Name = "Hot Stone Therapy", CategoryId = 1, Description = "Heated basalt stones placed on key points to melt away tension and restore energy flow, paired with a full-body massage.", DurationMinutes = 75, Price = 150m, IsActive = true },
            new Service { Id = 10, Name = "Reflexology", CategoryId = 1, Description = "Pressure-point therapy on the feet and hands to restore balance and overall wellbeing.", DurationMinutes = 45, Price = 85m, IsActive = true },
            new Service { Id = 11, Name = "Prenatal Massage", CategoryId = 1, Description = "A gentle, side-lying massage tailored to the needs of expecting mothers, easing pregnancy-related tension safely.", DurationMinutes = 60, Price = 130m, IsActive = true },
            new Service { Id = 12, Name = "Couples Retreat", CategoryId = 1, Description = "Side-by-side Swedish massages in our private couples suite, complete with champagne.", DurationMinutes = 90, Price = 280m, IsActive = true },
            new Service { Id = 13, Name = "Volcanic Mud Wrap", CategoryId = 3, Description = "Mineral-rich volcanic mud draws out impurities and leaves skin glowing and refreshed.", DurationMinutes = 60, Price = 130m, IsActive = true },
            new Service { Id = 14, Name = "Detox Salt Scrub", CategoryId = 3, Description = "A full-body exfoliation using mineral salts to remove dead skin and stimulate circulation.", DurationMinutes = 45, Price = 90m, IsActive = true },
            new Service { Id = 15, Name = "Express Facial", CategoryId = 2, Description = "A quick refresh - cleanse, exfoliate, and hydrate for guests short on time.", DurationMinutes = 30, Price = 70m, IsActive = true },
            new Service { Id = 16, Name = "Hydrating Facial", CategoryId = 2, Description = "A moisture-replenishing treatment for dry or dehydrated skin, leaving it soft and dewy.", DurationMinutes = 50, Price = 110m, IsActive = true },
            new Service { Id = 17, Name = "Hydrating Manicure", CategoryId = 4, Description = "A nourishing manicure with a moisturizing soak, shaping, cuticle care, and polish.", DurationMinutes = 30, Price = 55m, IsActive = true },
            new Service { Id = 18, Name = "Gel Manicure", CategoryId = 4, Description = "A long-lasting, chip-resistant gel polish manicure with full nail prep.", DurationMinutes = 45, Price = 65m, IsActive = true },
            new Service { Id = 19, Name = "Classic Pedicure", CategoryId = 4, Description = "A relaxing foot soak, exfoliation, nail shaping, and polish.", DurationMinutes = 45, Price = 60m, IsActive = true },
            new Service { Id = 20, Name = "Deluxe Spa Pedicure", CategoryId = 4, Description = "An extended pedicure with a warm paraffin treatment and calf massage.", DurationMinutes = 60, Price = 85m, IsActive = true },
            new Service { Id = 21, Name = "Sauna Session", CategoryId = 5, Description = "Private access to our dry sauna to relax muscles and promote detoxification.", DurationMinutes = 30, Price = 25m, IsActive = true },
            new Service { Id = 22, Name = "Steam Room Access", CategoryId = 5, Description = "Private access to our steam room to open pores and ease respiratory tension.", DurationMinutes = 30, Price = 20m, IsActive = true },
            new Service { Id = 23, Name = "Scalp & Head Massage (Add-On)", CategoryId = 5, Description = "Add a soothing scalp and head massage to any service.", DurationMinutes = 15, Price = 20m, IsActive = true }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, FirstName = "Sarah", LastName = "Mitchell", Email = "sarah.mitchell@example.com", PasswordHash = "seed-no-login", Phone = "555-0201", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 2, FirstName = "James", LastName = "Whitfield", Email = "james.whitfield@example.com", PasswordHash = "seed-no-login", Phone = "555-0202", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 3, FirstName = "Priya", LastName = "Anand", Email = "priya.anand@example.com", PasswordHash = "seed-no-login", Phone = "555-0203", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            // Demo-only clients and staff for the Client Service Notes reference
            // data below. PasswordHash = "seed-no-login" (same as the clients
            // above) so these accounts can never actually sign in. IDs start at
            // 100 to avoid colliding with whatever real accounts already exist
            // on a dev's local database.
            new User { Id = 100, FirstName = "Lena", LastName = "Fischer", Email = "lena.fischer@example.com", PasswordHash = "seed-no-login", Phone = "555-0301", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 101, FirstName = "Robert", LastName = "Chen", Email = "robert.chen@example.com", PasswordHash = "seed-no-login", Phone = "555-0302", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 102, FirstName = "Megan", LastName = "Delgado", Email = "megan.delgado@example.com", PasswordHash = "seed-no-login", Phone = "555-0303", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 103, FirstName = "Tobias", LastName = "Brandt", Email = "tobias.brandt@example.com", PasswordHash = "seed-no-login", Phone = "555-0304", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 104, FirstName = "Yuki", LastName = "Tanaka", Email = "yuki.tanaka@example.com", PasswordHash = "seed-no-login", Phone = "555-0305", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 105, FirstName = "Devon", LastName = "Cole", Email = "devon.cole@example.com", PasswordHash = "seed-no-login", Phone = "555-0401", Role = "Provider", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 106, FirstName = "Priya", LastName = "Raman", Email = "priya.raman@example.com", PasswordHash = "seed-no-login", Phone = "555-0402", Role = "Provider", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 107, FirstName = "Marcus", LastName = "Whitfield", Email = "marcus.whitfield@example.com", PasswordHash = "seed-no-login", Phone = "555-0403", Role = "Provider", CreatedAt = new DateTime(2026, 7, 1) },
            new User { Id = 108, FirstName = "Sofia", LastName = "Lindqvist", Email = "sofia.lindqvist@example.com", PasswordHash = "seed-no-login", Phone = "555-0404", Role = "Provider", CreatedAt = new DateTime(2026, 7, 1) }
        );

        modelBuilder.Entity<Testimonial>().HasData(
            new Testimonial { Id = 1, ClientId = 1, Rating = 5, ReviewText = "This was exactly what I needed after months of desk work. My shoulders finally feel loose again.", ApprovalStatus = "Approved", CreatedAt = new DateTime(2026, 7, 10) },
            new Testimonial { Id = 2, ClientId = 2, Rating = 5, ReviewText = "My skin has never looked better. The esthetician really listened to what I wanted.", ApprovalStatus = "Approved", CreatedAt = new DateTime(2026, 7, 15) },
            new Testimonial { Id = 3, ClientId = 3, Rating = 4, ReviewText = "Such a relaxing experience from start to finish. I'll definitely be back.", ApprovalStatus = "Approved", CreatedAt = new DateTime(2026, 7, 20) }
        );

        modelBuilder.Entity<ServiceNote>().HasData(
            new ServiceNote { Id = 1, ClientId = 100, AuthorId = 105, NoteType = "Allergy", NoteText = "Severe allergy to lavender oil — avoid all lavender-based products.", CreatedAt = new DateTime(2026, 7, 20) },
            new ServiceNote { Id = 2, ClientId = 100, AuthorId = 105, NoteType = "Preference", NoteText = "Prefers firm pressure and warm room temperature.", CreatedAt = new DateTime(2026, 7, 20) },
            new ServiceNote { Id = 3, ClientId = 101, AuthorId = 106, NoteType = "Allergy", NoteText = "Mild sensitivity to fragrances; use unscented products.", CreatedAt = new DateTime(2026, 7, 20) },
            new ServiceNote { Id = 4, ClientId = 102, AuthorId = 107, NoteType = "General", NoteText = "Recovering from shoulder injury — avoid deep pressure on right shoulder.", CreatedAt = new DateTime(2026, 7, 20) },
            new ServiceNote { Id = 5, ClientId = 103, AuthorId = 106, NoteType = "Preference", NoteText = "Likes the calming playlist and dim lighting.", CreatedAt = new DateTime(2026, 7, 20) },
            new ServiceNote { Id = 6, ClientId = 104, AuthorId = 108, NoteType = "Allergy", NoteText = "Allergic to nuts — check all product ingredient lists.", CreatedAt = new DateTime(2026, 7, 20) }
        );
    }
}