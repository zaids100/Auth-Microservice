using Microsoft.EntityFrameworkCore;
using System;
namespace AuthService.Models;

public class BookNowDbContext : DbContext
{

    public DbSet<User> Users { get; set; }

    public BookNowDbContext(DbContextOptions<BookNowDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.GoogleId).HasMaxLength(255);
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.GoogleId).IsUnique();
        });

        modelBuilder.Entity<User>().HasData(
      new User
      {
          Id = "U004",
          Email = "admin@system.com",
          PasswordHash = "sdhfsdkjf",
          GoogleId = null,
          Provider = "local",
          Role = "admin",
          CreatedAt = new DateTime(2025, 06, 01),  
          UpdatedAt = new DateTime(2025, 06, 01)   
      },
       new User
       {
           Id = "U005",
           Email = "organiser@system.com",
           PasswordHash = "sdfhsdfk",
           GoogleId = null,
           Provider = "local",
           Role = "organiser",
           CreatedAt = new DateTime(2025, 06, 01),
           UpdatedAt = new DateTime(2025, 06, 01)
       },
        new User
        {
            Id = "U006",
            Email = "customer@system.com",
            PasswordHash = "sdfhsdkf",
            GoogleId = null,
            Provider = "local",
            Role = "customer",
            CreatedAt = new DateTime(2025, 06, 01),
            UpdatedAt = new DateTime(2025, 06, 01)
        }
   );

    }
}
