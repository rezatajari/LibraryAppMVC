# Library Management System

A minimalist, full-stack web application built using the **KISS (Keep It Simple, Stupid)** principle. This system manages library operations including user registration, authentication, books directory, and borrowing logs, utilizing **ASP.NET Core MVC/Web API** for the backend and **Blazor WebAssembly** for the frontend client.

---

## Key Features

* **Secure Authentication & Registration:** Direct user registration with raw password hashing using native .NET `PasswordHasher`.
* **Reactive Client-Side Route Guard:** Custom router configuration in Blazor utilizing `CascadingAuthenticationState` to block unauthorized access to core management links.
* **Dynamic UI Navigation:** Context-aware navigation menu that reveals management options only to authenticated users, with a fast local-storage logout workflow.
* **Books & Users Directories:** Full CRUD capability for tracking library inventory and active members.

---

## Domain Entities (Database Models)

The core business domain consists of simple, decoupled entities shared across both the API and client layers:

### 1. User
Represents a library member or system operator.
* `Id` (int, Primary Key)
* `FullName` (string, Required)
* `Email` (string, Required, Unique)
* `PhoneNumber` (string, Optional)
* `Password` (string, Required, Stored as secure hash)
* `MembershipDate` (DateTime, Defaulted to current time)

### 2. Book
Tracks individual assets inside the library catalog.
* `Id` (int, Primary Key)
* `Title` (string)
* `Author` (string)
* `YearPublished` (int)
* `IsAvailable` (bool)

---

## System Architecture & Views

### Backend API (`UsersController`)
Handles lightweight and direct database interaction without intermediate DTO overhead for rapid prototyping:
* `POST /api/users/register` - Validates email uniqueness and hashes raw passwords before persistence.
* `POST /api/users/login` - Computes cryptographic hash verification to initialize sessions.
* `POST /api/users/logout` - Endpoint placeholder to trigger secure client-side token/session disposal.

### Frontend Pages (Blazor WebAssembly)
* **`/login` (Authentication Hub):** Dual-purpose streamlined form that toggles seamlessly between Login and Registration states.
* **`/dashboard`:** Landing space for registered users.
* **`/books`:** Authorized system view to inspect, search, and manage book assets.
* **`/users`:** Authorized control board to inspect and edit library user credentials.

---

## Technology Stack

* **Backend:** .NET 8.0 / 9.0 Core Web API, Entity Framework Core (EF Core)
* **Frontend:** Blazor WebAssembly (WASM), Bootstrap 5, Bootstrap Icons
* **State Management:** Browser Local Storage via `IJSRuntime` Interop

---

## Getting Started

1. Apply migrations to update your local database:
   ```bash
   dotnet ef database update