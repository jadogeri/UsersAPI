
## **🌐 UsersAPIModern RESTful User Management Service**

**Author:** Joseph Adogeri

**Version:** 1.0.0

**Date:** March 30, 2026

---

## Description

A production-ready .NET 9 Web API designed for secure user management. This system implements industry-standard security using Argon2id password hashing, a robust Repository Pattern, and a global exception handling middleware to ensure consistent, secure, and reliable API interactions.## Authors

- [joseph adogeri](https://www.github.com/jadogeri) 

## 📋 Table of Contents

<ul>
    <li><a href="#project-overview">🏗 Project Overview</a> </li>
    <li><a href="#architecture">🏛 Architecture</a>
		<ul>
        	<li><a href="#six-layered-service">6-Tier Layered Service Architecture Diagram </a></li>
        	<li><a href="#class-diagram">🧩 Class Diagram </a></li>
	        <li><a href="#folder-structure">📁 Project Folder Structure </a></li>
		</ul>
    </li>
    <li><a href="#workflow-diagrams">⚙️ Workflow & Interaction Diagrams</a></li>
    <li><a href="#security-features">🔐 Security Features</a></li>
    <li><a href="#api-reference">🚀 Service Endpoints & API Reference</a>
		<ul>
			<li><a href="#GetAssessmentUpdateStatus">1. GetAssessmentUpdateStatus</a></li>
		</ul> 
	</li>
    <li><a href="#technology-stack">💎 Technology Stack</a> </li>

    <li><a href="#getting-started">🚀 Getting Started</a> 
		<ol type="1">
        	<li><a href="#service-reference">Service Reference (Connected Services)</a></li>
        	<li><a href="#file-setup">File Setup</a></li>
	        <li><a href="#configuration">Configuration</a></li>
			<li><a href="#security-and-production">Security & Production</a></li>
		</ol>
	</li>
    <li><a href="#testing-the-service">🧪 Testing the Service</a>
		<ul>
			<li><a href="#testing-explorer">Running Tests via Test Explorer
</a></li>
        	<li><a href="#debugging-test">Debugging a Specific Test</a></li>
        	<li><a href="#testing-example">Example Test Structure</a></li>
		</ul>
	</li>
    <li><a href="#ci-cd-pipeline">🔄 CI/CD Pipeline</a> </li>
</ul>

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
    A["<div style='color:#01579b'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 🏠 Hosting Layer &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>"] --> B["<div style='color:#b71c1c'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 🛡️ Security Layer &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>"]
    B --> C["<div style='color:#e65100'>&nbsp;&nbsp;&nbsp; 📡 Service Interface &nbsp;&nbsp;&nbsp;</div>"]
    C --> D["<div style='color:#4a148c'>&nbsp;&nbsp;&nbsp;&nbsp; 🧠 Business Logic &nbsp;&nbsp;&nbsp;&nbsp;</div>"]
    D --> E["<div style='color:#1b5e20'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 💾 Data Access &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>"]
    E --> F[("<div style='color:#263238'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 🗄️ Database &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div>")]

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
        << (🌐) API Controller >>
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
        << (🧠) Service >>
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
        << (📦) Abstract >>
        #TContext _context
        #DbSet~T~ _dbSet
        +GetAllAsync() Task~IEnumerable~
        +FindByIdAsync(int id) Task~T~
        +AddAsync(T) Task
        +UpdateAsync(T) Task
        +DeleteAsync(int id) Task
    }
    class UserRepository {
        << (💾) Repository >>
    }

    %% --- LAYER 4: MODELS & DATA (GRAY) ---
    class DatabaseContext {
        << (🗄️) EF Core >>
        +DbSet~User~ Users
    }
    class User {
        << (📦) Entity >>
        +int Id
        +string Username
        +string Email
        +string PasswordHash
    }
    class CreateUserDto { <<DTO>> }
    class UpdateUserDto { <<DTO>> }

    %% --- RELATIONSHIPS ---
    IUserService <|.. UserService : «implements»
    IUserRepository <|.. UserRepository : «implements»
    BaseRepository <|-- UserRepository : «inherits»
    
    %% Injections (Aggregation)
    UsersController o-- IUserService : «use»
    UserService o-- IUserRepository : «use»
    UserRepository o-- DatabaseContext : «uses»

    %% Data Flow (Dependency)
    UserService ..> User : «creates/maps»
    UsersController ..> CreateUserDto : «receives»
    UsersController ..> UpdateUserDto : «receives»

    %% --- STYLING ---
    style UsersController fill:#fff3e0,stroke:#e65100,color:#000
    style UserService fill:#f3e5f5,stroke:#4a148c,color:#000
    style UserRepository fill:#e8f5e9,stroke:#1b5e20,color:#000
    style DatabaseContext fill:#eceff1,stroke:#263238,color:#000
    style User fill:#f5f5f5,stroke:#9e9e9e,color:#000
    style IUserService fill:#f3e5f5,stroke:#4a148c,color:#000
    style IUserRepository fill:#e8f5e9,stroke:#1b5e20,color:#000
    %% FIXED: Referred to the class name without generic tildes for styling
    style BaseRepository fill:#e8f5e9,stroke:#1b5e20,color:#000
    style CreateUserDto fill:#f5f5f5,stroke:#9e9e9e,color:#000
    style UpdateUserDto fill:#f5f5f5,stroke:#9e9e9e,color:#000
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

