# MVCApp - E-Commerce Web Application

A modern e-commerce web application built with ASP.NET Core 8.0 MVC, featuring product catalog browsing, shopping cart functionality, and secure Stripe payment integration.

## 🌐 Live Demo

**Website:** [http://a-mart.runasp.net/](http://a-mart.runasp.net/)  
**Video Demo:** [Watch on YouTube](https://youtu.be/qXTHfVEdaOY)

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

### 1. Configure Connection String

Update `appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MVCApp;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 2. Configure Stripe Keys

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

### 3. Configure App URL

Set your application URL for Stripe redirects:

```json
{
  "AppUrl": "https://localhost:5001"
}
```

## 🚀 Getting Started

### Run the Application

```bash
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

## 🧪 Default Test Credentials

| User | Email | Password |
|------|-------|----------|
| Admin | admin@ecommerce.com | Admin123! |

## 📝 Logging

Logs are written to:
- **Console** - Development debugging
- **File** - `logs/app-{date}.txt` with daily rolling

## 📄 License

This project is licensed under the MIT License.
