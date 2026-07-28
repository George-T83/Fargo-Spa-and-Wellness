using Family_and_Spa_Wellness.Models;
using Microsoft.EntityFrameworkCore;

namespace Family_and_Spa_Wellness.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Service> Services => Set<Service>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Deep Tissue Massage", Category = "Massage", Description = "Targeted pressure to release deep muscle tension.", DurationMinutes = 90, Price = 145m },
            new Service { Id = 2, Name = "Swedish Relaxation Massage", Category = "Massage", Description = "A gentle, flowing massage to ease everyday stress.", DurationMinutes = 60, Price = 110m },
            new Service { Id = 3, Name = "Aromatherapy Body Wrap", Category = "Body", Description = "Nourishing botanical wrap with essential oils.", DurationMinutes = 75, Price = 120m },
            new Service { Id = 4, Name = "Body Polish & Buff", Category = "Body", Description = "A gentle, all-over exfoliating polish that leaves skin smooth, soft, and radiant.", DurationMinutes = 50, Price = 100m },
            new Service { Id = 5, Name = "Signature Facial", Category = "Facial & Skincare", Description = "Customized facial for radiant, glowing skin.", DurationMinutes = 60, Price = 95m },
            new Service { Id = 6, Name = "Anti-Aging Collagen Facial", Category = "Facial & Skincare", Description = "A collagen-boosting treatment targeting fine lines and loss of elasticity.", DurationMinutes = 60, Price = 130m },
            new Service { Id = 7, Name = "Lavender Relaxation Ritual", Category = "Wellness", Description = "A full-body relaxation journey with lavender.", DurationMinutes = 120, Price = 180m },
            new Service { Id = 8, Name = "Aromatherapy Enhancement", Category = "Wellness", Description = "Add a custom essential oil blend to any massage or body treatment.", DurationMinutes = 15, Price = 25m }
        );
    }
}
