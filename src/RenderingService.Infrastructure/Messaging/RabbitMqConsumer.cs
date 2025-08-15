using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RenderingService.Application.Handlers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RenderingService.Infrastructure.Messaging;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly RabbitMQ.Client.IModel _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqConsumer> _logger;

    public RabbitMqConsumer(IConnection connection, IServiceProvider serviceProvider, ILogger<RabbitMqConsumer> logger)
    {
        _connection = connection;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _channel = _connection.CreateModel();

        SetupQueues();
    }

    private void SetupQueues()
    {
        _channel.ExchangeDeclare("forms.events", ExchangeType.Topic, true);

        // Queue for form published events
        _channel.QueueDeclare("rendering.formpublished", true, false, false, null);
        _channel.QueueBind("rendering.formpublished", "forms.events", "forms.formpublishedevent");

        // Queue for form updated events
        _channel.QueueDeclare("rendering.formupdated", true, false, false, null);
        _channel.QueueBind("rendering.formupdated", "forms.events", "forms.formupdatedevent");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var publishedConsumer = new EventingBasicConsumer(_channel);
        publishedConsumer.Received += async (ch, ea) =>
        {
            await HandleFormPublishedEvent(ea);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        var updatedConsumer = new EventingBasicConsumer(_channel);
        updatedConsumer.Received += async (ch, ea) =>
        {
            await HandleFormUpdatedEvent(ea);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume("rendering.formpublished", false, publishedConsumer);
        _channel.BasicConsume("rendering.formupdated", false, updatedConsumer);

        _logger.LogInformation("RabbitMQ consumer started");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleFormPublishedEvent(BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventData = JsonSerializer.Deserialize<FormPublishedEvent>(message);

            if (eventData != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<FormPublishedHandler>();
                await handler.HandleAsync(eventData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing FormPublished event");
        }
    }

    private async Task HandleFormUpdatedEvent(BasicDeliverEventArgs ea)
    {
        try
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var eventData = JsonSerializer.Deserialize<FormUpdatedEvent>(message);

            if (eventData != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<FormUpdatedHandler>();
                await handler.HandleAsync(eventData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing FormUpdated event");
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}