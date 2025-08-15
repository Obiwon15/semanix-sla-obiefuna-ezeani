# Semanix SLA Solution

## Overview
The Semanix SLA solution is a .NET-based application for rendering forms, built with a clean architecture approach. It includes a Web API, application logic, domain models, infrastructure services, and comprehensive testing.

## Project Structure
- **src/RenderingService.Web**: ASP.NET Core Web API for handling HTTP requests.
- **src/RenderingService.Application**: Application logic, including handlers and queries.
- **src/RenderingService.Domain**: Domain models and business logic.
- **src/RenderingService.Infrastructure**: Infrastructure services, including messaging and persistence.
- **tests/RenderingService.UnitTests**: Unit tests for application logic.
- **tests/RenderingService.IntegrationTests**: Integration tests for the solution.
- **docs**: Architectural documentation and diagrams.

## Setup Instructions
1. **Prerequisites**:
   - .NET SDK 8.0.303
   - Visual Studio 2022
   - Docker (optional, for containerization)

2. **Clone the Repository**:
   `Bash
   git clone <repository-url>
   cd semanix-sla-obiefuna-ezeani
   `

3. **Build the Solution**:
   `Bash
   dotnet build semanix-sla-obiefuna-ezeani.sln
   `

4. **Run the Web API**:
   `Bash
   cd src/RenderingService.Web
   dotnet run
   `

5. **Run Tests**:
   `Bash
   dotnet test semanix-sla-obiefuna-ezeani.sln
   `

## Architecture
See docs/Architecture.md for a detailed description and docs/ArchitectureDiagram.puml for a visual representation.

## Contributing
- Follow the .editorconfig for code style.
- Add unit and integration tests for new features.
- Update documentation in the docs directory.
