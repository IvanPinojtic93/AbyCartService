using CartService.Application.Common.Interfaces;
using CartService.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CartService.Infrastructure.Events;

public sealed class RabbitMqPublisher : IEventPublisher, IDisposable
{
    private readonly IConnection? _connection;
    private readonly IModel? _channel;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IOptions<RabbitMqSettings> opts, ILogger<RabbitMqPublisher> logger)
    {
        _logger = logger;
        try
        {
            var s = opts.Value;
            var factory = new ConnectionFactory
            {
                HostName = s.Host,
                Port = s.Port,
                UserName = s.Username,
                Password = s.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("webshop.events", ExchangeType.Topic, durable: true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not connect to RabbitMQ on startup");
        }
    }

    public void TryPublish(string routingKey, object payload)
    {
        if (_channel is null)
        {
            _logger.LogWarning("RabbitMQ unavailable; skipping publish of {Key}", routingKey);
            return;
        }
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;
            _channel.BasicPublish("webshop.events", routingKey, props, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {Key}", routingKey);
        }
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
