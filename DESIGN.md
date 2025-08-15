### DESIGN.md
```Markdown
# Design Rationale

## Architectural Decisions

### Clean Architecture + CQRS
The solution implements Clean Architecture with clear separation of concerns:
- **Domain**: Pure business logic, no external dependencies
- **Application**: Use cases, command/query handlers
- **Infrastructure**: Data access, external services
- **Presentation**: Controllers, middleware

CQRS separates read and write operations:
- Commands use Entity Framework for consistency
- Queries use Dapper for performance
- Different models optimized for their purpose

### Event Sourcing & Messaging
Forms Service publishes domain events (FormPublished, FormUpdated) to RabbitMQ. The Rendering Service consumes these events to maintain its read model. This ensures:
- **Loose coupling**: Services don't directly communicate
- **Audit trail**: Complete history of form changes
- **Scalability**: Easy to add new consumers
- **Reliability**: Message persistence and retry mechanisms

### State Machine Implementation
Form lifecycle (Draft ? Published ? Archived) is enforced in the domain layer with explicit validation. Invalid transitions throw domain exceptions, ensuring data integrity.

### Multi-tenancy
X-Tenant-Id header is mandatory and validated at the middleware level. All data is tenant-scoped, ensuring complete isolation.

## Technology Choices

### .NET 8 & Entity Framework Core
- **Performance**: Significant improvements in .NET 8
- **Tooling**: Excellent development experience
- **Migrations**: Automatic schema management

### RabbitMQ
- **Reliability**: Message persistence and acknowledgments
- **Routing**: Flexible exchange/queue configuration
- **Monitoring**: Built-in management interface
- **Scalability**: Clustering support

### Dapper for Queries
- **Performance**: Minimal overhead over ADO.NET
- **Control**: Fine-grained SQL optimization
- **Flexibility**: Complex queries without ORM limitations

## Scalability Considerations

### High-Volume Tenants

**Horizontal Scaling**
- Stateless services enable easy horizontal scaling
- Load balancer distributes requests across instances
- Database connection pooling prevents resource exhaustion

**Database Partitioning**
- Partition forms table by tenant_id hash
- Separate read replicas for query workloads
- Consider tenant-specific databases for largest customers

**Event Processing**
- RabbitMQ clustering for message throughput
- Partition events by tenant for parallel processing
- Dead letter queues for failed message handling

**Caching Strategy**
- Redis cache for frequently accessed forms
- CDN for static form assets
- Application-level caching for reference data

**Performance Optimizations**
- Database indexing on tenant_id, form_id
- Async processing where possible
- Background services for non-critical operations

**Monitoring & Alerting**
- Per-tenant metrics and dashboards
- SLA monitoring and alerting
- Automatic scaling based on load

The event-driven architecture naturally supports these scaling patterns, allowing each service to scale independently based on its specific load patterns.
