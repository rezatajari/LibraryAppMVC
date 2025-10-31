# LibraryAppMVC

A modern ASP.NET Core MVC web application for managing a library system, featuring user authentication, book management, JWT-based APIs, and robust logging.

---

## 🌐 Live Demo

🔗 [Visit Live Website](http://rezatajari-001-site1.qtempurl.com/)

---

## Application Demo  
### 1. User Panel
![Initial Setup](https://github.com/rezatajari/LibraryAppMVC/blob/master/Demo.png)  
*Book Management panel*

---

## Features

- User registration, login, and profile management (with email confirmation)
- Book CRUD operations (add, list, search, details)
- JWT authentication for secure API access
- Role-based access control
- Email notifications (SMTP)
- Logging with Serilog (console and SQL Server)
- Responsive Razor views

---

## Folder Structure

```
├── Controllers/      # MVC controllers (Account, Library, Home)
├── Data/             # Entity Framework DbContext
├── Interfaces/       # Service and repository interfaces
├── Migrations/       # EF Core migrations
├── Models/           # Entity models (User, Book, etc.)
├── Repositories/     # Data access repositories
├── Services/         # Business logic and utilities (JWT, Email, etc.)
├── Utilities/        # Helper classes
├── ViewModels/       # View models for MVC
├── Views/            # Razor views (Account, Library, Home, Shared)
├── wwwroot/          # Static files (css, js, images)
├── appsettings.json  # Main configuration
├── Program.cs        # Application entry point
├── LibraryAppMVC.csproj # Project file
```

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- SQL Server (local or remote)

### Setup
1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/LibraryAppMVC.git
   cd LibraryAppMVC
   ```
2. **Configure the database:**
   - Update the `ConnectionStrings:LibraryDB` in `appsettings.json` with your SQL Server details.
3. **Configure SMTP (for email):**
   - Update the `SMTP` section in `appsettings.json` with your email provider credentials.
4. **Configure JWT (optional):**
   - Adjust the `JwtSettings` in `appsettings.json` as needed.
5. **Apply migrations:**
   ```bash
   dotnet ef database update
   ```
6. **Run the application:**
   ```bash
   dotnet run
   ```
   The app will be available at `https://localhost:5001` (or as configured).

---

## Usage
- Register a new user (email confirmation required)
- Login and manage your profile
- Add, view, search, and manage books
- Admins can manage users and books

---

## Technologies Used
- ASP.NET Core MVC (.NET 9)
- Entity Framework Core (SQL Server)
- ASP.NET Identity
- Serilog (logging)
- Razor Views

---

## Project Structure Details
- **Controllers/**: Handle HTTP requests and responses
- **Models/**: Define data entities (Book, User, etc.)
- **Services/**: Business logic (BookService, AccountService, JwtService, EmailSender)
- **Repositories/**: Data access logic (BookRepository)
- **ViewModels/**: Data transfer objects for views
- **Views/**: UI (Razor pages for Account, Library, Home, Shared)
- **Utilities/**: Helper classes (e.g., ResultTask)
- **Data/**: EF Core DbContext (LibraryDB)

---

## Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## New Version (coming soon)
Now I choose to build new version of this application like blow structure

                   ┌─────────────────────────────┐
                   │         Angular App          │
                   │ (SPA, Routing, Material UI)  │
                   └──────────────┬───────────────┘
                                  │ REST / HTTPS
                   ┌──────────────┴───────────────┐
                   │        .NET 8 Web API        │
                   │ (Clean Architecture Layers)  │
                   │   Application / Domain / DB  │
                   └──────────────┬───────────────┘
                                  │ EF Core
                            ┌─────┴─────┐
                            │ SQLServer │
                            └───────────┘

