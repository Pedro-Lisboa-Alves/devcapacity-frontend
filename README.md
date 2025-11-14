# DevCapacity Web App

Small Razor Pages web front-end for the DevCapacity API. Shows engineers, teams, tasks and a timeline calendar view of engineer availability/assignments.

## Quick facts
- Type: ASP.NET Core Razor Pages application
- Framework: .NET 7 (ASP.NET Core 7)
- UI: Bootstrap 5 (CDN)
- Language: C#

## Features
- CRUD for Engineers, Teams, Tasks
- Engineer calendar and day types (Available, Vacations, Weekends, Absence, Assigned)
- Timeline view: matrix of engineers × days (month-by-month) with pagination
- Assignment management with validation (no duplicate assignment per task; obey Task.MaxResources)
- Server-side pagination and filtering
- Dockerfile and docker-compose example for local container runs

## Technical details

Supported .NET / runtime
- Target framework: .NET 7 (net7.0)
- SDK images used in Dockerfile examples: `mcr.microsoft.com/dotnet/sdk:7.0` and `mcr.microsoft.com/dotnet/aspnet:7.0`

Key dependencies (NuGet / built-in)
- Microsoft.AspNetCore.App (Razor Pages)
- System.Net.Http.Json (HTTP JSON helpers)
- Microsoft.AspNetCore.Mvc.RazorPages
- Bootstrap 5 (included via CDN in layout)

Client-side
- Minimal JS for sidebar toggle and Bootstrap bundle
- Validation scripts partial (_ValidationScriptsPartial) for form validation

API client
- DevCapacityApiClient wraps HttpClient; configured via DI in `Program.cs`
- Base API URL configurable via environment variable `DEV_CAPACITY_API_BASE` (recommended for docker)

## Project layout (important files/folders)
- DevCapacityWebApp/ — Razor Pages app
  - Pages/ — Razor Pages (Engineers, Teams, Tasks, Timeline, Shared partials)
  - Models/ — DTOs and view models (EngineerDto, EngineerCalendarDayDto, Tasks, Team, etc.)
  - Services/DevCapacityApiClient.cs — API client for backend calls
  - wwwroot/ — static assets (css/site.css)
  - Program.cs — app startup and DI configuration
  - Dockerfile — multi-stage build example
  - .dockerignore
- README.md — this file
- LICENSE — MIT License

## Running locally

Prerequisites:
- .NET 7 SDK installed
- (Optional) Docker

Run with dotnet:
1. Open terminal in `DevCapacityWebApp` folder:
   cd "/Users/pedroalves/Desktop/Tmp Work/GitHub Projects/devcapacity-frontend/DevCapacityWebApp"
2. Set API base (if backend running locally on 5001):
   export DEV_CAPACITY_API_BASE="http://localhost:5001/"
3. Run:
   dotnet run

Run in Docker:
1. Build image:
   docker build -t devcapacity-frontend:local .
2. Run container and map port:
   docker run --rm -p 5000:80 -e DEV_CAPACITY_API_BASE="http://host.docker.internal:5001/" devcapacity-frontend:local
3. Open http://localhost:5000

Or use provided docker-compose (if present):
   docker-compose up --build

Notes:
- If backend API runs in another container, set `DEV_CAPACITY_API_BASE` to the API container name and port (e.g. `http://api:5000/`).
- Use `host.docker.internal` on macOS to reach host services from container.

## Configuration
- Environment variable: `DEV_CAPACITY_API_BASE` — sets HttpClient BaseAddress used by the app.
- If not provided, Program.cs reads appsettings fallback values.

## Contributing
- Open pull requests with clear description.
- Keep UI/UX changes responsive (Bootstrap 5 conventions used).
- Run `dotnet build` and test pages before PR.

## License
This project is licensed under the MIT License — see the [LICENSE](./LICENSE) file for details.
