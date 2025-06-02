using System.Text;
using System.Text.Json;
using NotificationService.Api.Models;
using NotificationService.Api.Services.EmailService;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationService.Api.Services.MessageConsumerService;

public class MessageConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IEmailService _emailService;
    private const string QueueName = "email_queue";

    public MessageConsumer(IConfiguration configuration, IEmailService emailService)
    {
        _emailService = emailService;

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"],
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"]
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<EmailMessage>(json);

            if (message != null)
            {
                await _emailService.SendEmailAsync(message);
            }
        };

        _channel.BasicConsume(
            queue: QueueName,
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}