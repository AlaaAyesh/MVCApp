using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCApp.Models;

namespace MVCApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // Create admin user
            var adminEmail = "admin@ecommerce.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and gadgets", IsActive = true },
                    new Category { Name = "Clothing", Description = "Fashion and apparel", IsActive = true },
                    new Category { Name = "Books", Description = "Books and literature", IsActive = true },
                    new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true },
                    new Category { Name = "Sports", Description = "Sports equipment and accessories", IsActive = true }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Seed products
            if (!await context.Products.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Wireless Bluetooth Headphones",
                        Description = "High-quality wireless headphones with noise cancellation",
                        Price = 99.99m,
                        SalePrice = 79.99m,
                        StockQuantity = 50,
                        CategoryId = categories.First(c => c.Name == "Electronics").Id,
                        IsActive = true,
                        IsFeatured = true,
                        SKU = "WH-001"
                    },
                    new Product
                    {
                        Name = "Smartphone Case",
                        Description = "Durable protective case for smartphones",
                        Price = 29.99m,
                        StockQuantity = 100,
                        CategoryId = categories.First(c => c.Name == "Electronics").Id,
                        IsActive = true,
                        SKU = "SC-001"
                    },
                    new Product
                    {
                        Name = "Cotton T-Shirt",
                        Description = "Comfortable cotton t-shirt in various colors",
                        Price = 19.99m,
                        SalePrice = 14.99m,
                        StockQuantity = 200,
                        CategoryId = categories.First(c => c.Name == "Clothing").Id,
                        IsActive = true,
                        IsFeatured = true,
                        SKU = "TS-001"
                    },
                    new Product
                    {
                        Name = "Denim Jeans",
                        Description = "Classic denim jeans with perfect fit",
                        Price = 59.99m,
                        StockQuantity = 75,
                        CategoryId = categories.First(c => c.Name == "Clothing").Id,
                        IsActive = true,
                        SKU = "DJ-001"
                    },
                    new Product
                    {
                        Name = "Programming Book",
                        Description = "Comprehensive guide to modern programming",
                        Price = 49.99m,
                        SalePrice = 39.99m,
                        StockQuantity = 30,
                        CategoryId = categories.First(c => c.Name == "Books").Id,
                        IsActive = true,
                        SKU = "PB-001"
                    },
                    new Product
                    {
                        Name = "Garden Tool Set",
                        Description = "Complete set of essential garden tools",
                        Price = 89.99m,
                        StockQuantity = 25,
                        CategoryId = categories.First(c => c.Name == "Home & Garden").Id,
                        IsActive = true,
                        IsFeatured = true,
                        SKU = "GT-001"
                    },
                    new Product
                    {
                        Name = "Yoga Mat",
                        Description = "Non-slip yoga mat for home workouts",
                        Price = 34.99m,
                        StockQuantity = 60,
                        CategoryId = categories.First(c => c.Name == "Sports").Id,
                        IsActive = true,
                        SKU = "YM-001"
                    },
                    new Product
                    {
                        Name = "Running Shoes",
                        Description = "Comfortable running shoes for all terrains",
                        Price = 129.99m,
                        SalePrice = 99.99m,
                        StockQuantity = 40,
                        CategoryId = categories.First(c => c.Name == "Sports").Id,
                        IsActive = true,
                        IsFeatured = true,
                        SKU = "RS-001"
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
} 