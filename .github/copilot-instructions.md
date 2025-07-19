# Copilot Instructions for Stavbe Codebase

## Overview
This repository is a full-stack solution for managing building (stavbe) data, with a .NET API backend and an Angular client frontend. The workspace is organized into two main folders: `API/` (C#/.NET) and `client/` (Angular). Data flows from the backend (Entity Framework, REST controllers) to the frontend (Angular services/components).

## Key Architectural Patterns
- **Backend (API/):**
  - Uses Entity Framework Core for data access (`Data/`, `Entities/`, `Migrations/`).
  - Business logic and data queries are in `Data/Repos/` (e.g., `MojElektroRepository.cs`).
  - DTOs in `DTOs/` are used for shaping API responses.
  - Controllers in `Controllers/` expose REST endpoints.
  - Helpers, middleware, and extensions are in their respective folders for cross-cutting concerns.
- **Frontend (client/):**
  - Angular 18+ project structure (`src/app/`).
  - Models in `_models/` mirror backend DTOs/entities for type safety.
  - UI logic is split into feature modules/components (e.g., `moj-elektro-card`).
  - Uses `ngx-bootstrap` for UI widgets (e.g., datepickers/tabs).

## Developer Workflows
- **Build/Run:**
  - Backend: Use Visual Studio or `dotnet build`, run with `dotnet run`.
  - Frontend: Use `ng serve` for development, `ng build` for production builds.
- **Testing:**
  - Backend: Standard .NET test projects (not present in context, add if needed).
  - Frontend: Run `ng test` for unit tests (Karma).
- **Debugging:**
  - Backend: Use Visual Studio debugger or `dotnet watch run`.
  - Frontend: Use browser dev tools and Angular CLI error output.

## Project-Specific Conventions
- **DTOs and Entities:**
  - Always use DTOs for API responses; do not expose entities directly.
  - Map entities to DTOs using AutoMapper (see `Helpers/AutoMapperProfiles.cs`).
- **Repository Pattern:**
  - All data access is via repository classes in `Data/Repos/`.
  - Async methods return `Task<T>` and use EF Core's async LINQ methods.
- **Frontend Data Binding:**
  - Use Angular's async pipes and RxJS for API calls.
  - UI components expect strongly typed models from `_models/`.
- **UI Libraries:**
  - For datepickers/tabs, import `ngx-bootstrap` modules in your Angular module (see error troubleshooting above).

## Integration Points
- **API <-> Client:**
  - REST endpoints in `Controllers/` are consumed by Angular services.
  - Data contracts are kept in sync via DTOs and TypeScript interfaces.
- **External Dependencies:**
  - Angular: `ngx-bootstrap`, `@types/google.maps` (for map features).
  - API: Entity Framework Core, AutoMapper.

## Examples
- **Adding a new API endpoint:**
  - Create a DTO in `DTOs/`, update the entity in `Entities/`, add logic in `Data/Repos/`, expose via a controller in `Controllers/`.
- **Adding a new Angular feature:**
  - Generate a component (`ng generate component`), add a model in `_models/`, create a service for API calls, and update routing/module as needed.

## Key Files/Folders
- `API/Controllers/` - REST API endpoints
- `API/Data/Repos/` - Data access logic
- `API/DTOs/` - Data transfer objects
- `client/src/app/_models/` - TypeScript models
- `client/src/app/` - Angular components/services

---

If any section is unclear or missing important project-specific details, please provide feedback or specify which workflows/patterns need more documentation.
