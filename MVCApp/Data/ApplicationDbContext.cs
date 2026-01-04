using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCApp.Models;
using Microsoft.AspNetCore.Identity;

namespace MVCApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Category configuration
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.imageUrl).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Product configuration
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SalePrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.imageUrl).HasMaxLength(500);
                entity.Property(e => e.SKU).HasMaxLength(50);
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.HasOne(e => e.Category)
                    .WithMany(e => e.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Order configuration
            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ShippingAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.StripePaymentIntentId).HasMaxLength(100);
                entity.Property(e => e.StripeSessionId).HasMaxLength(100);
                entity.Property(e => e.ShippingFirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ShippingLastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ShippingAddress).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ShippingCity).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ShippingState).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ShippingZipCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ShippingCountry).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ShippingPhone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.ShippingEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem configuration
            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.imageUrl).HasMaxLength(500);
                entity.HasOne(e => e.Order)
                    .WithMany(e => e.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Product)
                    .WithMany(e => e.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
