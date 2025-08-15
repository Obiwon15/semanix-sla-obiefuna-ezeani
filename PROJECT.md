Semanix Forms Engine Project Overview

Project Purpose

The Semanix Forms Engine is a distributed, scalable forms management system designed to handle form creation, validation, lifecycle management, and rendering for multi-tenant applications. Built with Clean Architecture, CQRS, and Event Sourcing, it ensures modularity, testability, and performance. The solution comprises two microservices: FormsService for managing form lifecycles and RenderingService for providing read-optimized form data to front-end clients.

Objectives



Enable dynamic form creation with customizable sections, fields, and validation rules.

Support multi-tenancy with strict data isolation.

Provide high-performance querying and scalability through event-driven architecture.

Ensure reliability with robust error handling, logging, and health checks.

Facilitate easy setup and deployment using Docker and .NET 8.



Project Structure

The solution is organized as follows:



src/FormsService: Manages form creation, validation, and lifecycle (Draft → Published → Archived).

Domain: Core entities (Form.cs), value objects (FormId.cs, Section.cs), and events (FormPublishedEvent.cs).

Application: CQRS commands (CreateFormCommand.cs) and queries (GetFormQuery.cs).

Infrastructure: EF Core persistence (FormsDbContext.cs), Dapper queries (FormQueryService.cs), RabbitMQ messaging (RabbitMqEventPublisher.cs).

Web: REST API (FormsController.cs) with tenant validation middleware.





src/RenderingService: Consumes form events to maintain a read-optimized model.

Domain: RenderedForm.cs for read models.

Application: Handles events (FormPublishedHandler.cs) and queries (GetRenderedFormsQuery.cs).

Infrastructure: EF Core persistence (RenderingDbContext.cs), RabbitMQ consumer (RabbitMqConsumer.cs).

Web: REST API (RenderedFormsController.cs).





tests: Unit tests (FormsService.Tests, RenderingService.Tests) and integration tests (FormsControllerTests.cs, RenderingIntegrationTests.cs).

Root Files:

README.md: Setup and usage instructions.

DESIGN.md: Architectural decisions and rationale.

docker-compose.yml: Orchestrates services and dependencies.

.dockerignore, .gitignore: Exclude unnecessary files.

semanix-forms.sln: Solution file.







Architecture

The solution follows Clean Architecture with CQRS and Event Sourcing:



Clean Architecture: Separates concerns into Domain, Application, Infrastructure, and Web layers.

CQRS: Commands (EF Core) for writes, queries (Dapper) for reads.

Event Sourcing: Form events (FormPublishedEvent, FormUpdatedEvent) are published to RabbitMQ, consumed by RenderingService for read model updates.

Multi-tenancy: Enforced via X-Tenant-Id header and tenant-scoped data.

Technologies:

.NET 8 for performance and tooling.

EF Core for migrations and writes, Dapper for query performance.

RabbitMQ for reliable event-driven communication.

SQL Server for persistent storage.







Setup and Usage

Prerequisites



.NET SDK 8.0.303

Docker and Docker Compose

Visual Studio 2022 (optional for local development)



Quick Start



Clone the repository:git clone <repository-url>

cd semanix-sla-forms





Start services:docker-compose up -d





Verify health:curl http://localhost:5000/health  # FormsService

curl http://localhost:5001/health  # RenderingService





Access services:

FormsService: http://localhost:5000/swagger

RenderingService: http://localhost:5001/swagger

RabbitMQ: http://localhost:15672 (user: admin, password: admin)







Example API Usage



Create a form:curl -X POST http://localhost:5000/api/forms -H "Content-Type: application/json" -H "X-Tenant-Id: tenant-123" -H "X-Entity-Id: entity-456" -d '{"name":"Employee Onboarding","description":"New hire checklist","sections":\[{"name":"Personal Information","order":1,"fields":\[{"fieldId":"full-name","label":"Full Name","type":"Text","order":1,"validationRules":{"required":true,"maxLength":100}}]}]}'





Publish a form:curl -X POST http://localhost:5000/api/forms/{form-id}/publish -H "X-Tenant-Id: tenant-123"





Get rendered forms:curl "http://localhost:5001/api/rendered-forms?tenant=tenant-123"







Development



Run Tests:dotnet test





Apply Migrations (when database is available):cd src/FormsService

dotnet ef database update

cd src/RenderingService

dotnet ef database update





Local Development:docker-compose up rabbitmq sqlserver

cd src/FormsService \&\& dotnet run

cd src/RenderingService \&\& dotnet run







Monitoring



Health Checks: /health endpoints for both services.

Logging: Serilog for structured logging.

Metrics: fe\_forms\_published\_total counter for form publications.

API Documentation: OpenAPI at /swagger endpoints.



