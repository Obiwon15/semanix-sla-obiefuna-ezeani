# RenderingService Architecture

## Overview
The RenderingService follows a clean architecture pattern, separating concerns into layers: Web, Application, Domain, and Infrastructure. This ensures modularity, testability, and maintainability.

## Layers
- **Web**: Handles HTTP requests and responses using ASP.NET Core Web API. Contains controllers (RenderedFormsController.cs) and middleware (ExceptionHandlingMiddleware.cs).
- **Application**: Contains business logic, including command/query handlers (FormPublishedHandler.cs, FormUpdatedHandler.cs, GetRenderedFormsQuery.cs) and service interfaces (IRenderingService.cs).
- **Domain**: Defines core business models (RenderedForm.cs) and entities.
- **Infrastructure**: Implements external services, including messaging (RabbitMqConsumer.cs), persistence (RenderingDbContext.cs), and service implementations (RenderingService.cs).
- **Tests**: Includes unit tests (RenderingHandlerTests.cs) and integration tests (RenderingIntegrationTests.cs).

## Communication
- The Web layer exposes RESTful endpoints, calling Application layer handlers.
- The Application layer orchestrates business logic, interacting with Domain models and Infrastructure services.
- Infrastructure uses RabbitMQ for messaging and Entity Framework Core for persistence.

## Diagram
See ArchitectureDiagram.puml for a visual representation (use PlantUML to render).

## Design Principles
- **Single Responsibility**: Each layer has a distinct role.
- **Dependency Injection**: Used throughout to manage dependencies.
- **Testability**: Unit and integration tests cover all layers.
