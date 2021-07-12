using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Beerka.Persistence
{
    public class BeerkaContext : IdentityDbContext<Employee>
    {
        public DbSet<MainCategory> MainCategories { get; set; }

        public DbSet<SubCategory> SubCategories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductOrder> ProductOrders { get; set; }

        public BeerkaContext(DbContextOptions<BeerkaContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Setting composite key for ProductOrder:
            modelBuilder.Entity<ProductOrder>().HasKey(po => new { po.ProductID, po.OrderID });

            // Ensuring main categories uniqueness:
            modelBuilder.Entity<MainCategory>().HasIndex(c => c.Name).IsUnique(true);

            // Ensuring subcategories uniqueness:
            modelBuilder.Entity<SubCategory>().HasIndex(c => new { c.Name, c.MainCategoryID }).IsUnique(true);

            // Ensuring products uniqueness:
            modelBuilder.Entity<Product>().HasIndex(p => new
                {
                    p.Name,
                    p.Manufacturer,
                    p.ModelNumber,
                    p.SubCategoryID,
                    p.Description
                }).IsUnique(true);

            modelBuilder.Entity<Product>()
                .Property(typeof(string), "_packagingTypeString");

            base.OnModelCreating(modelBuilder);
        }

    }
}
