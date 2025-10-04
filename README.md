
# 🛒 E-Commerce Web Application

A full-featured E-Commerce web application built with ASP.NET Core MVC that provides complete online shopping functionality including product browsing, shopping cart management, order processing, and comprehensive admin panel.

## 🚀 Features

### 👤 Admin Features
- **Authentication:** Login with admin credentials
- **Product Management:** Create, read, update, delete products
- **Category Management:** Manage product categories
- **User Account Management:** View and manage customer accounts
- **Order Management:** Track and update order status
- **Dashboard:** Overview of system statistics

### 🧑 Customer Features
- **User Authentication:** Secure login and registration system
- **Product Catalog:** Browse products by categories
- **Product Details:** View detailed product information
- **Shopping Cart:** Add, remove, and update items
- **Checkout Process:** Complete order placement
- **Order History:** View past orders
- **Session Management:** Persistent shopping cart

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core 8.0 MVC
- **Database:** SQL Server with Entity Framework Core 9.0
- **Frontend:** HTML5, CSS3, Bootstrap 5, JavaScript
- **Authentication:** Cookie-based authentication
- **Session Management:** In-memory distributed cache
- **Architecture:** MVC Pattern with Repository-like structure

## 📊 Database Schema

The application uses the following main entities:
- **KhachHang** (Customers): User accounts and profiles
- **HangHoa** (Products): Product catalog with categories
- **Loai** (Categories): Product categorization
- **HoaDon** (Orders): Customer orders
- **ChiTietHd** (Order Details): Individual order items
- **TrangThai** (Order Status): Order status tracking

## 📦 Setup & Installation

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/quoctuan-IT/E-Commerce.git
   cd E-Commerce
   ```

2. **Update connection string**
   Create `appsettings.json` and update the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ECommerce;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   - Open browser and navigate to `https://localhost:5001`
   - Admin panel: Login with credentials above
   - Customer features: Register a new account

## 🎯 Project Structure

```
E-Commerce/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── CartController.cs
│   ├── HomeController.cs
│   └── ProductController.cs
├── Models/              # Data Models
│   ├── HangHoa.cs       # Product model
│   ├── KhachHang.cs     # Customer model
│   ├── HoaDon.cs        # Order model
│   └── ViewModels/      # View-specific models
├── Views/               # Razor Views
│   ├── Home/           # Homepage views
│   ├── Product/        # Product views
│   ├── Cart/           # Shopping cart views
│   ├── Account/        # Authentication views
│   └── Admin/          # Admin panel views
├── Data/               # Database context
│   └── AppDbContext.cs
├── Helpers/            # Utility classes
│   ├── AutoMapperProfile.cs
│   ├── MyUtil.cs
│   └── SessionExtensions.cs
└── wwwroot/            # Static files
    ├── css/            # Stylesheets
    ├── js/             # JavaScript files
    └── hinh/           # Product images
```

## 🔧 Configuration

- **Session Timeout:** 10 minutes (configurable in Program.cs)
- **Authentication:** Cookie-based with automatic redirects
- **File Upload:** Product images stored in wwwroot/hinh/HangHoa/
- **Database:** SQL Server with Entity Framework migrations

## 🚀 Getting Started

1. **For Customers:**
   - Register a new account
   - Browse products by category
   - Add items to cart
   - Proceed to checkout
   - Complete order

2. **For Administrators:**
   - Login with admin credentials
   - Access admin dashboard
   - Manage products, categories, and orders
   - Monitor system activity

## 📝 Notes

- The application uses Vietnamese naming conventions for database entities
- Product images are stored locally in the wwwroot directory
- Session-based shopping cart persists during user session
- Admin and customer roles are managed through the VaiTro field in KhachHang table
