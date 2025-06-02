using System.Text;
using System.Text.Json;
using NotificationService.Api.Models;
using RabbitMQ.Client;

namespace NotificationService.Api.Services.EmailPublisherService;

public class EmailPublisherService : IEmailPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "email_queue";

    public EmailPublisherService(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"],
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"]
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public void PublishMessage(EmailMessage message)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: "",
            routingKey: QueueName,
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}