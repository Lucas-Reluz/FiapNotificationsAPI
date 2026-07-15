using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationsAPI.Domain.Events;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Enums;
using NotificationsAPI.Domain.Interfaces;

namespace NotificationsAPI.Infrastructure.Messaging;

public class OrderEventConsumer : BackgroundService
{
    private readonly ILogger<OrderEventConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public OrderEventConsumer(
        ILogger<OrderEventConsumer> logger,
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            var exchangeName = "payment.exchange";
            await _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var queueName = "notifications.payment.queue";
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: string.Empty,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Mensagem recebida: {Message}", message);

                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                    var jsonDoc = JsonDocument.Parse(message);
                    var root = jsonDoc.RootElement;

                    if (root.TryGetProperty("OrderId", out _) && root.TryGetProperty("ConfirmedAt", out _))
                    {
                        var orderConfirmedEvent = JsonSerializer.Deserialize<OrderConfirmedEvent>(message);
                        if (orderConfirmedEvent != null)
                        {
                            var notification = new Notification(
                                userId: orderConfirmedEvent.UserId,
                                orderId: orderConfirmedEvent.OrderId,
                                type: NotificationType.OrderConfirmed,
                                title: "Pedido Confirmado",
                                message: $"Seu pedido do jogo '{orderConfirmedEvent.GameTitle}' foi confirmado com sucesso!");

                            await repository.AddAsync(notification);
                            _logger.LogInformation("Notificação OrderConfirmed criada para UserId {UserId}", orderConfirmedEvent.UserId);
                        }
                    }
                    else if (root.TryGetProperty("OrderId", out _) && root.TryGetProperty("CancelledAt", out _))
                    {
                        var orderCancelledEvent = JsonSerializer.Deserialize<OrderCancelledEvent>(message);
                        if (orderCancelledEvent != null)
                        {
                            var notification = new Notification(
                                userId: orderCancelledEvent.UserId,
                                orderId: orderCancelledEvent.OrderId,
                                type: NotificationType.OrderCancelled,
                                title: "Pedido Cancelado",
                                message: $"Seu pedido do jogo '{orderCancelledEvent.GameTitle}' foi cancelado. Motivo: {orderCancelledEvent.CancellationReason}");

                            await repository.AddAsync(notification);
                            _logger.LogInformation("Notificação OrderCancelled criada para UserId {UserId}", orderCancelledEvent.UserId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem do RabbitMQ");
                }
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken);

            _logger.LogInformation("OrderEventConsumer iniciado e aguardando mensagens de {Exchange}", exchangeName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar OrderEventConsumer");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
