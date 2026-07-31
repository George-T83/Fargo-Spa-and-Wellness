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

        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Deep Tissue Massage", Category = "Massage", Description = "Targeted pressure to release deep muscle tension.", DurationMinutes = 90, Price = 145m, IsActive = true },
            new Service { Id = 2, Name = "Swedish Relaxation Massage", Category = "Massage", Description = "A gentle, flowing massage to ease everyday stress.", DurationMinutes = 60, Price = 110m, IsActive = true },
            new Service { Id = 3, Name = "Aromatherapy Body Wrap", Category = "Body", Description = "Nourishing botanical wrap with essential oils.", DurationMinutes = 75, Price = 120m, IsActive = true },
            new Service { Id = 4, Name = "Body Polish & Buff", Category = "Body", Description = "A gentle, all-over exfoliating polish that leaves skin smooth, soft, and radiant.", DurationMinutes = 50, Price = 100m, IsActive = true },
            new Service { Id = 5, Name = "Signature Facial", Category = "Facial & Skincare", Description = "Customized facial for radiant, glowing skin.", DurationMinutes = 60, Price = 95m, IsActive = true },
            new Service { Id = 6, Name = "Anti-Aging Collagen Facial", Category = "Facial & Skincare", Description = "A collagen-boosting treatment targeting fine lines and loss of elasticity.", DurationMinutes = 60, Price = 130m, IsActive = true },
            new Service { Id = 7, Name = "Lavender Relaxation Ritual", Category = "Wellness", Description = "A full-body relaxation journey with lavender.", DurationMinutes = 120, Price = 180m, IsActive = true },
            new Service { Id = 8, Name = "Aromatherapy Enhancement", Category = "Wellness", Description = "Add a custom essential oil blend to any massage or body treatment.", DurationMinutes = 15, Price = 25m, IsActive = true }
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