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
            new User { Id = 3, FirstName = "Priya", LastName = "Anand", Email = "priya.anand@example.com", PasswordHash = "seed-no-login", Phone = "555-0203", Role = "Client", CreatedAt = new DateTime(2026, 7, 1) }
        );

        modelBuilder.Entity<Testimonial>().HasData(
            new Testimonial { Id = 1, ClientId = 1, Rating = 5, ReviewText = "This was exactly what I needed after months of desk work. My shoulders finally feel loose again.", ApprovalStatus = "Approved", CreatedAt = new DateTime(2026, 7, 10) },
            new Testimonial { Id = 2, ClientId = 2, Rating = 5, ReviewText = "My skin has never looked better. The esthetician really listened to what I wanted.", ApprovalStatus = "Approved", CreatedAt = new DateTime(2026, 7, 15) },
            new Testimonial { Id = 3, ClientId = 3, Rating = 4, ReviewText = "Such a relaxing experience from start to finish. I'll definitely be back.", ApprovalStatus = "Approved", CreatedAt = new DateTime(2026, 7, 20) }
        );
    }
}