# SkillSnap - Portfolio and Project Tracker

A full-stack application built with ASP.NET Core Web API and Blazor WebAssembly for managing your portfolio, projects, and skills.

## Project Structure

- **SkillSnap.Api** - ASP.NET Core Web API backend with Entity Framework Core and SQLite
- **SkillSnap.Client** - Blazor WebAssembly frontend application

## Features

- Portfolio user management
- Project tracking with CRUD operations
- Skill management with proficiency levels (Beginner, Intermediate, Advanced)
- RESTful API with CORS enabled
- Interactive Blazor UI components

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Terminal or command prompt

### Running the Application

1. **Start the API** (Terminal 1):
   ```bash
   cd SkillSnap.Api
   dotnet run
   ```
   The API will start at:
   - HTTPS: https://localhost:7279
   - HTTP: http://localhost:5196

2. **Seed the Database** (one-time setup):
   ```bash
   curl -X POST https://localhost:7279/api/seed
   ```
   Or use a browser/Postman to POST to `https://localhost:7279/api/seed`

3. **Start the Blazor Client** (Terminal 2):
   ```bash
   cd SkillSnap.Client
   dotnet run
   ```
   The client will start at:
   - HTTPS: https://localhost:7043
   - HTTP: http://localhost:5090

4. Open your browser to https://localhost:7043

## API Endpoints

### Projects
- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get project by ID
- `POST /api/projects` - Create new project
- `PUT /api/projects/{id}` - Update project
- `DELETE /api/projects/{id}` - Delete project

### Skills
- `GET /api/skills` - Get all skills
- `GET /api/skills/{id}` - Get skill by ID
- `POST /api/skills` - Create new skill
- `PUT /api/skills/{id}` - Update skill
- `DELETE /api/skills/{id}` - Delete skill

### Seed Data
- `POST /api/seed` - Insert sample data (Jordan Developer with 2 projects and 2 skills)

## Technologies Used

- ASP.NET Core 9.0
- Blazor WebAssembly
- Entity Framework Core
- SQLite
- Bootstrap 5

## Project Progress

### Part 1: Scaffolding ✅
- Created data models
- Configured EF Core with SQLite
- Built static Blazor components

### Part 2: API Integration ✅
- Built API controllers with full CRUD operations
- Enabled CORS for client-server communication
- Created Blazor services for HTTP communication
- Connected UI components to data sources
- Added forms for creating new projects and skills

## Development Notes

- Database file: `SkillSnap.Api/skillsnap.db`
- The database is automatically created on first run
- CORS is configured to allow requests from the Blazor client
- All services use dependency injection
