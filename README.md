# 📚 BulkyBook - E-Commerce Bookstore

A modern full-stack e-commerce web application built with **ASP.NET Core MVC**, following clean architecture principles and best practices.

This project was built as part of my ASP.NET Core learning journey and demonstrates authentication, authorization, payment processing, repository pattern, and deployment-ready architecture.

---

# 🚀 Technologies

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Repository Pattern
- Unit of Work Pattern
- Bootstrap 5
- JavaScript
- Stripe Payment Gateway
- Mailjet Email Service
- Azure Deployment Ready

---

# ✨ Features

### Customer

- Browse books
- Search products
- Add products to cart
- Checkout using Stripe
- View order history

### Admin

- Dashboard
- Manage Products
- Manage Categories
- Manage Companies
- Manage Users
- Manage Orders
- Assign Roles

### Authentication

- Register
- Login
- Forgot Password
- Email Confirmation
- Role-based Authorization

---

# 🏗️ Project Architecture

```
BulkyBook
│
├── BulkyBookWeb
├── BulkyBook.Models
├── BulkyBook.DataAccess
├── BulkyBook.Business
└── BulkyBook.Utility
```

---

# 🛠️ Getting Started

## 1. Clone Repository

```bash
git clone https://github.com/youssef-sameh4/BulkyBook---E-Commerce-Bookstore.git
```

## 2. Open Solution

Open the solution using **Visual Studio 2022**.

---

## 3. Configure Database

Update your connection string inside:

```
appsettings.json
```

Example:

```json
"ConnectionStrings": {
    "DefaultConnection": "Your SQL Server Connection String"
}
```

---

## 4. Configure Stripe

```json
"Stripe": {
    "SecretKey": "YOUR_SECRET_KEY",
    "PublishableKey": "YOUR_PUBLISHABLE_KEY"
}
```

---

## 5. Configure Mailjet

```json
"MailJet": {
    "ApiKey": "YOUR_API_KEY",
    "SecretKey": "YOUR_SECRET_KEY"
}


# 📖 What I Learned

- ASP.NET Core MVC
- Entity Framework Core
- Repository & Unit of Work Pattern
- Identity Authentication
- Authorization
- Stripe Payment Integration
- Mailjet Email Service
- Azure Deployment
- Dependency Injection
- Session Management
- View Components
- Areas
- Clean Architecture

---

# 📌 Future Improvements

- Product Reviews
- Wishlist
- Product Rating
- Search Filters
- REST API
- JWT Authentication
- Docker Support

---

# 👨‍💻 Author

**Yousef Elazab**

GitHub

https://github.com/youssef-sameh4


# ⭐ Acknowledgments

This project was developed while following the **Complete ASP.NET Core MVC** course by **Bhrugen Patel**, with additional customization and implementation during my learning journey.
