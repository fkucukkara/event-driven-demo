using System.Text;
using System.Text.Json;
using EventDrivenDemo.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventDrivenDemo.Shared.Messaging;

/// <summary>
/// Background service for consuming events from RabbitMQ
/// </summary>
public class RabbitMqEventConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEventConsumer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly Dictionary<string, Type> _eventTypes;

    public RabbitMqEventConsumer(
        IServiceProvider serviceProvider,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqEventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        _eventTypes = new Dictionary<string, Type>
        {
            { "OrderCreatedEvent", typeof(OrderCreatedEvent) },
            { "OrderUpdatedEvent", typeof(OrderUpdatedEvent) },
            { "OrderCancelledEvent", typeof(OrderCancelledEvent) }
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken); // Wait for RabbitMQ to be ready

        try
        {
            InitializeRabbitMq();

            // Keep the service running until cancellation is requested
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
            _logger.LogInformation("RabbitMQ event consumer is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RabbitMQ event consumer");
            throw; // Re-throw to ensure the service fails properly
        }
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Set QoS to process one message at a time
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        // Declare exchange
        _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true);

        // Declare queue for this service
        var queueName = $"{Environment.GetEnvironmentVariable("SERVICE_NAME") ?? "unknown"}.queue";
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

        // Bind to all order events
        _channel.QueueBind(queueName, _options.ExchangeName, "ordercreated");
        _channel.QueueBind(queueName, _options.ExchangeName, "orderupdated");
        _channel.QueueBind(queueName, _options.ExchangeName, "ordercancelled");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                await ProcessMessage(ea, CancellationToken.None);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message: {Error}", ex.Message);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queueName, false, consumer);
        _logger.LogInformation("Started consuming events from queue {QueueName}", queueName);
    }

    private async Task ProcessMessage(BasicDeliverEventArgs ea, CancellationToken cancellationToken)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var eventType = ea.BasicProperties.Type;

        _logger.LogDebug("Processing message of type {EventType}", eventType);

        if (!_eventTypes.TryGetValue(eventType, out var type))
        {
            _logger.LogWarning("Unknown event type: {EventType}", eventType);
            return;
        }

        var eventObject = JsonSerializer.Deserialize(message, type, _jsonOptions);
        if (eventObject == null)
        {
            _logger.LogWarning("Failed to deserialize event of type {EventType}", eventType);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IEventHandler<>).MakeGenericType(type);
        var handler = scope.ServiceProvider.GetService(handlerType);

        if (handler != null)
        {
            var method = handlerType.GetMethod("HandleAsync");
            if (method != null)
            {
                await (Task)method.Invoke(handler, new[] { eventObject, cancellationToken })!;
                _logger.LogDebug("Successfully processed event {EventType}", eventType);
            }
        }
        else
        {
            _logger.LogDebug("No handler found for event type {EventType}", eventType);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
