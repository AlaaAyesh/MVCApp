# MVCApp - E-Commerce Web Application

A modern e-commerce web application built with ASP.NET Core 8.0 MVC, featuring product catalog browsing, shopping cart functionality, and secure Stripe payment integration.

## 🚀 Features

- **Product Catalog** - Browse products with search, filtering, sorting, and pagination
- **Category Management** - Products organized by categories (Electronics, Clothing, Books, etc.)
- **Shopping Cart** - Add, update, and remove items with real-time price calculations
- **Secure Checkout** - Integrated Stripe payment processing
- **Order Management** - Track order status and history
- **User Authentication** - Register, login, and role-based authorization
- **Admin Panel** - Product CRUD operations for administrators
- **Responsive Design** - Bootstrap-based UI for all devices

## 🛠️ Tech Stack

| Technology | Purpose |
|------------|---------|
| ASP.NET Core 8.0 MVC | Web framework |
| Entity Framework Core 8.0 | ORM / Data Access |
| SQL Server | Database |
| ASP.NET Core Identity | Authentication & Authorization |
| Stripe.net | Payment processing |
| AutoMapper | Object-to-object mapping |
| FluentValidation | Input validation |
| Serilog | Structured logging |
| Bootstrap 5 | Frontend styling |
| jQuery | Client-side interactivity |

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- [Stripe Account](https://stripe.com/) (for payment processing)

## ⚙️ Configuration

### 1. Clone the Repository

```bash
git clone <repository-url>
cd MVCApp
```

### 2. Configure Connection String

Update `appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MVCApp;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Configure Stripe Keys

Add your Stripe API keys to `appsettings.json`:

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_your_publishable_key",
    "SecretKey": "sk_test_your_secret_key",
    "WebhookSecret": "whsec_your_webhook_secret"
  }
}
```

> ⚠️ **Security Note:** For production, use [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables instead of storing keys in `appsettings.json`.

### 4. Configure App URL

Set your application URL for Stripe redirects:

```json
{
  "AppUrl": "https://localhost:5001"
}
```

## 🚀 Getting Started

### Run the Application

```bash
cd MVCApp
dotnet restore
dotnet run
```

The application will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`

### Database Initialization

The database is automatically created and seeded on first run with:
- **Admin User:** `admin@ecommerce.com` / `Admin123!`
- **Sample Categories:** Electronics, Clothing, Books, Home & Garden, Sports
- **Sample Products:** 8 demo products across categories

## 📁 Project Structure

```
MVCApp/
├── Controllers/          # MVC Controllers
│   ├── AuthController.cs       # Authentication endpoints
│   ├── CartController.cs       # Shopping cart operations
│   ├── HomeController.cs       # Home page
│   ├── OrderController.cs      # Order management
│   └── ProductsController.cs   # Product catalog & admin
├── Data/                 # Data layer
│   ├── ApplicationDbContext.cs # EF Core DbContext
│   └── SeedData.cs            # Database seeding
├── Models/               # Domain models
│   ├── Category.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── Product.cs
├── ViewModels/           # View models for MVC views
├── Services/             # Business logic & API services
│   ├── ApiAuthService.cs      # Authentication API client
│   ├── ApiCartService.cs      # Cart API client
│   ├── ApiCategoryService.cs  # Category API client
│   ├── ApiOrderService.cs     # Order API client
│   ├── ApiProductService.cs   # Product API client
│   └── StripeService.cs       # Stripe payment integration
├── Repositories/         # Repository pattern implementation
├── Mapping/              # AutoMapper profiles
├── Views/                # Razor views
│   ├── Auth/                  # Login & Register views
│   ├── Cart/                  # Cart & Checkout views
│   ├── Home/                  # Home & Privacy views
│   ├── Order/                 # Order tracking views
│   ├── Products/              # Product listing & details
│   └── Shared/                # Layout & partial views
└── wwwroot/              # Static files (CSS, JS, images)
```

## 🔐 User Roles

| Role | Capabilities |
|------|-------------|
| **Admin** | Full access: manage products, view all orders |
| **Customer** | Browse products, manage cart, place orders, view own orders |
| **Guest** | Browse products only |

## 🔄 API Integration

This MVC application integrates with a backend REST API for:
- Product and category data
- Cart management
- Order processing
- User authentication (JWT tokens)

Session-based JWT token storage is used to maintain authentication state.

## 📝 Logging

Logs are written to:
- **Console** - Development debugging
- **File** - `logs/app-{date}.txt` with daily rolling

Configure log levels in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

## 🧪 Default Test Credentials

| User | Email | Password |
|------|-------|----------|
| Admin | admin@ecommerce.com | Admin123! |

## 📦 NuGet Packages

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` - Identity with EF Core
- `Microsoft.EntityFrameworkCore.SqlServer` - SQL Server provider
- `Stripe.net` - Stripe API client
- `AutoMapper.Extensions.Microsoft.DependencyInjection` - AutoMapper DI
- `FluentValidation.AspNetCore` - Validation framework
- `Serilog.AspNetCore` - Structured logging
- `Serilog.Sinks.File` - File logging sink
- `Serilog.Sinks.Console` - Console logging sink

## 🚢 Deployment

### Publish the Application

```bash
dotnet publish -c Release -o ./publish
```

### Production Checklist

- [ ] Update connection strings for production database
- [ ] Configure production Stripe keys (live keys)
- [ ] Set `ASPNETCORE_ENVIRONMENT` to `Production`
- [ ] Enable HTTPS
- [ ] Configure proper CORS if needed
- [ ] Set up application insights or monitoring

## 📄 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

