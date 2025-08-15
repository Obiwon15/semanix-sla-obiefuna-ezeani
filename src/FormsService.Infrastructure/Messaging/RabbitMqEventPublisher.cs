using RabbitMQ.Client;
using FormsService.Application.Services;
using FormsService.Domain.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FormsService.Infrastructure.Messaging;

public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<RabbitMqEventPublisher> _logger;

    public RabbitMqEventPublisher(IConnection connection, ILogger<RabbitMqEventPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
        _channel = _connection.CreateModel();

        // Declare exchange
        _channel.ExchangeDeclare("forms.events", ExchangeType.Topic, true);
    }

    public async Task PublishDomainEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await PublishEventAsync(domainEvent, cancellationToken);
        }
    }

    private async Task PublishEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        try
        {
            var eventType = domainEvent.GetType().Name;
            var routingKey = $"forms.{eventType.ToLowerInvariant()}";

            var message = JsonSerializer.Serialize(domainEvent);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = domainEvent.Id.ToString();
            properties.Timestamp = new AmqpTimestamp(((DateTimeOffset)domainEvent.OccurredOn).ToUnixTimeSeconds());
            properties.Type = eventType;

            _channel.BasicPublish(
                exchange: "forms.events",
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published event {EventType} with ID {EventId}", eventType, domainEvent.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventType} with ID {EventId}", domainEvent.GetType().Name, domainEvent.Id);
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}