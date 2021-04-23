using eshop_api.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace eshop_api.Models
{
    public class EshopContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {

        public EshopContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ShoppingCartRecord> ShoppingCartRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // seed data

            builder.Entity<IdentityRole<int>>().HasData(
                    new IdentityRole<int>
                    {
                        Id = 1,
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new IdentityRole<int>
                    {
                        Id = 2,
                        Name = "Customer",
                        NormalizedName = "CUSTOMER"
                    }
                );

            var hasher = new PasswordHasher<AppUser>();
            var user = new AppUser
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@example.com",
                EmailConfirmed = true,
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                SecurityStamp = new Guid().ToString("D")
            };
            user.PasswordHash = hasher.HashPassword(user, "password");
            builder.Entity<AppUser>().HasData(user);

            builder.Entity<IdentityUserRole<int>>().HasData(
                    new IdentityUserRole<int>
                    {
                        UserId = 1,
                        RoleId = 1
                    }
                );

            // Fluent API settings

            builder.Entity<Customer>()
                .HasIndex(i => i.Email)
                .IsUnique();

            builder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ShipDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

            });

            builder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.LineItemTotal)
                    .HasColumnType("money")
                    .HasComputedColumnSql("[Quantity]*[UnitCost]");

                entity.Property(e => e.UnitCost)
                    .HasColumnType("money");
            });

            builder.Entity<Product>(entity =>
            {
                entity.Property(e => e.UnitCost).HasColumnType("money");
                entity.Property(e => e.CurrentPrice).HasColumnType("money");
            });

            builder.Entity<ShoppingCartRecord>(entity =>
            {
                entity.HasAlternateKey(c => new { c.Id, c.ProductId, c.CustomerId });
            });
        }
        public override int SaveChanges()
        {

            OnBeforeSaving();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entities = ChangeTracker
             .Entries()
             .Where(e => e.Entity is EntityBase &&
             (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entities)
            {
                ((EntityBase)entityEntry.Entity).UpdatedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((EntityBase)entityEntry.Entity).CreatedAt = DateTime.Now;
                }
            }
        }
    }
}
