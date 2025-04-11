using System;
using ColumnWiseSearch.Models;
using Microsoft.EntityFrameworkCore;

namespace ColumnWiseSearch.DataLibrary;

// DbContext
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
          new Product { Id = 1, Name = "Laptop", Description = "High performance laptop", Category = "Electronics", Price = 1200.00m, IsAvailable = true, CreatedDate = new DateTime(2023, 1, 15) },
          new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Category = "Electronics", Price = 800.00m, IsAvailable = true, CreatedDate = new DateTime(2023, 2, 20) },
          new Product { Id = 3, Name = "Headphones", Description = "Noise cancelling headphones", Category = "Audio", Price = 250.00m, IsAvailable = false, CreatedDate = new DateTime(2022, 12, 1) },
          new Product { Id = 4, Name = "Coffee Maker", Description = "Automatic coffee machine", Category = "Kitchen", Price = 150.00m, IsAvailable = true, CreatedDate = new DateTime(2023, 3, 5) }
      );
    }
}
