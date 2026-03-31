🌐 UsersAPIModern RESTful User Management Service
Author: Joseph Adogeri
Version: 1.0.0
Date: March 30, 2026
------------------------------
Description
A production-ready .NET 9 Web API designed for secure user management. This system implements industry-standard security using Argon2id password hashing, a robust Repository Pattern, and a global exception handling middleware to ensure consistent, secure, and reliable API interactions.
Authors

* Joseph Adogeri

------------------------------
📋 Table of Contents

🏗 Project Overview
🏛 Architecture

Layered Service Architecture Diagram
📁 Project Folder Structure


🔐 Security Features
🚀 Service Endpoints
💎 Technology Stack
🚀 Getting Started
🧪 Testing Suite
🔄 CI/CD Pipeline
------------------------------
🏗 Project Overview
This project provides a centralized API for user CRUD operations. It is built with a "Security-First" mindset, ensuring that sensitive data is never exposed and that the system is resilient against common attack vectors like brute-force (via Argon2id) and database race conditions.
🏛 Architecture
The system follows a strict separation of concerns, ensuring that the database logic, business rules, and HTTP transport layers remain decoupled.

| Layer | Responsibility | Components |
|---|---|---|
| 🏠 Hosting | Runtime & Dependency Injection | Program.cs, appsettings.json |
| 🛡️ Security | Hashing & Validation | Argon2id, GlobalExceptionHandler |
| 📡 Service Interface | Controller Layer (REST) | UsersController |
| 🧠 Business Logic | Validation & User Services | UserService |
| 💾 Data Access | Repository & Entity Framework | UserRepository, BaseRepository |
| 🗄️ Database | Persistence | SQL Server / InMemory |

Layered Service Architecture Diagram

```mermaid
graph TD
    A["&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 🏠 Hosting Layer &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"] --> B["&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 🛡️ Security Layer &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"]
    B --> C["&nbsp;&nbsp;&nbsp; 📡 Service Interface &nbsp;&nbsp;&nbsp;"]
    C --> D["&nbsp;&nbsp;&nbsp;&nbsp; 🧠 Business Logic &nbsp;&nbsp;&nbsp;&nbsp;"]
    D --> E["&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 💾 Data Access &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"]
    E --> F[("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 🗄️ Database &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")]

    style A fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    style B fill:#ffebee,stroke:#b71c1c,stroke-width:2px
    style C fill:#fff3e0,stroke:#e65100,stroke-width:2px
    style D fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    style E fill:#e8f5e9,stroke:#1b5e20,stroke-width:2px
    style F fill:#eceff1,stroke:#263238,stroke-width:2px
```

```mermaid
---
title: UsersAPI Class Diagram
---
classDiagram
    direction TB

    %% --- LAYER 1: SERVICE INTERFACE (ORANGE) ---
    class UsersController {
        << (🌐, #e65100) API Controller >>
        -IUserService _userService
        +GetAll() Task~ActionResult~
        +GetById(int id) Task~ActionResult~
        +Create(CreateUserDto) Task~ActionResult~
        +Update(int id, UpdateUserDto) Task~IActionResult~
        +Delete(int id) Task~IActionResult~
    }

    %% --- LAYER 2: BUSINESS LOGIC (PURPLE) ---
    class IUserService {
        <<interface>>
        +GetAllUsersAsync() Task~IEnumerable~
        +GetUserByIdAsync(int id) Task~User~
        +CreateUserAsync(CreateUserDto) Task~User~
        +UpdateUserAsync(int id, UpdateUserDto) Task~bool~
        +DeleteUserAsync(int id) Task~bool~
    }
    class UserService {
        << (🧠, #4a148c) Service >>
        -IUserRepository _userRepository
        -HashPassword(string) string
        +VerifyPassword(string, string) bool
    }

    %% --- LAYER 3: DATA ACCESS (GREEN) ---
    class IUserRepository {
        <<interface>>
        +FindByEmailAsync(string) Task~User~
        +FindByUsernameAsync(string) Task~User~
    }
    class BaseRepository~T~ {
        << (📦, #1b5e20) Abstract >>
        #TContext _context
        #DbSet~T~ _dbSet
        +GetAllAsync() Task~IEnumerable~
        +FindByIdAsync(int id) Task~T~
        +AddAsync(T) Task
        +UpdateAsync(T) Task
        +DeleteAsync(int id) Task
    }
    class UserRepository {
        << (💾, #1b5e20) Repository >>
    }

    %% --- LAYER 4: MODELS & DATA (GRAY) ---
    class DatabaseContext {
        << (🗄️, #263238) EF Core >>
        +DbSet~User~ Users
    }
    class User {
        << (📦, #9e9e9e) Entity >>
        +int Id
        +string Username
        +string Email
        +string PasswordHash
    }
    class CreateUserDto { <<DTO>> }
    class UpdateUserDto { <<DTO>> }

    %% --- RELATIONSHIPS ---
    IUserService <|.. UserService
    IUserRepository <|.. UserRepository
    BaseRepository <|-- UserRepository
    
    %% Injections (Aggregation)
    UsersController o-- IUserService : uses
    UserService o-- IUserRepository : uses
    UserRepository o-- DatabaseContext : uses

    %% Data Flow (Dependency)
    UserService ..> User : creates/maps
    UsersController ..> CreateUserDto : receives
    UsersController ..> UpdateUserDto : receives

    %% --- STYLING ---
    style UsersController fill:#fff3e0,stroke:#e65100
    style UserService fill:#f3e5f5,stroke:#4a148c
    style UserRepository fill:#e8f5e9,stroke:#1b5e20
    style DatabaseContext fill:#eceff1,stroke:#263238
    style User fill:#f5f5f5,stroke:#9e9e9e
    style IUserService fill:#f3e5f5,stroke:#4a148c
    style IUserRepository fill:#e8f5e9,stroke:#1b5e20

```

📁 Project Folder Structure

```
UsersAPI/
├── 📂 .github/
│   └── 📂 workflows/           # 🔄 CI/CD Pipeline (GitHub Actions)
├── 📂 UsersAPI/                # 🏠 Hosting Layer (Web API)
│   ├── 📂 Controllers/         # 📡 Service Interface (REST Endpoints)
│   ├── 📂 Data/                # 💾 Data Access Layer (EF Core Context)
│   ├── 📂 Models/              # 📦 Data Structures
│   │   ├── 📂 DTOs/            # 📩 Data Transfer Objects (Input/Output)
│   │   └── 📂 Entities/        # 🗄️ Database Domain Models
│   ├── 📂 Repositories/        # 🛠️ Repository Pattern (Base & User)
│   ├── 📂 Services/            # 🧠 Business Logic & Argon2id Hashing
│   ├── 📜 appsettings.json     # ⚙️ Configuration & Connection Strings
│   └── 📜 Program.cs           # 🚀 Entry Point & Dependency Injection
├── 📂 UsersAPI.Tests/          # 🧪 Testing Suite (NUnit)
│   ├── 📂 Controllers/         # 🚦 Integration Tests (WebApplicationFactory)
│   ├── 📂 Repositories/        # 💾 Persistence Tests (InMemory DB)
│   └── 📂 Services/            # 🧠 Logic & Security Tests (Moq)
├── 📜 .gitignore               # 🛑 Git Exclusion Rules
├── 📜 UsersAPI.slnx           # 💎 Modern Solution File
└── 📜 README.md                # 📖 Project Documentation
```
------------------------------
🔐 Security Features

   1. Argon2id Hashing: Uses Konscious.Security.Cryptography to provide memory-hard, side-channel resistant password hashing.
   2. Salt Generation: Cryptographically secure 16-byte unique salts per user.
   3. Global Exception Handling: Uses IExceptionHandler to intercept errors and return standardized ProblemDetails (RFC 7807) without leaking stack traces.
   4. Data Isolation: Uses DTOs (CreateUserDto, UpdateUserDto) to ensure internal entities and password hashes are never returned in API responses.

------------------------------
🚀 Service Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | /api/users | Retrieve all users |
| GET | /api/users/{id} | Retrieve a specific user by ID |
| POST | /api/users | Register a new user |
| PUT | /api/users/{id} | Update user profile |
| DELETE | /api/users/{id} | Remove a user |

------------------------------
💎 Technology Stack

* Runtime: .NET 9.0 (supporting .slnx solution format)
* Database: Entity Framework Core (SQL Server / InMemory)
* Cryptography: Argon2id
* Testing: NUnit, Moq, WebApplicationFactory
* CI/CD: GitHub Actions

------------------------------
🚀 Getting StartedPrerequisites

* .NET 9 SDK
* Visual Studio 2022 (v17.13+) or VS Code

Installation

   1. Clone the repository:
   
   git clone https://github.com
   
   2. Restore & Build:
   
   dotnet restore UsersAPI.slnx
   dotnet build UsersAPI.slnx
   
   3. Run the API:
   
   dotnet run --project UsersAPI/UsersAPI.csproj
   
   
------------------------------
🧪 Testing Suite
The project includes a comprehensive suite of 90+ tests (at least 30 per layer) divided into three categories:

* Happy Path: Standard successful operations.
* Edge Case: Handling empty strings, large IDs, and boundary values.
* Exception Case: Verifying conflict handling (duplicate emails) and database failures.

Run tests via CLI:

dotnet test UsersAPI.Tests/UsersAPI.Tests.csproj

------------------------------
🔄 CI/CD Pipeline
The project uses GitHub Actions to automate the build and test cycle.

* Trigger: Every push or PR to main.
* Environment: Ubuntu Linux using .NET 9 SDK.
* Steps: Checkout -> Setup .NET -> Restore -> Build -> Test.

Would you like me to add a "Project Folder Structure" section with a tree view of your files?

