using NotificationService.Api.Models;
using NotificationService.Api.Services;
using NotificationService.Api.Services.EmailPublisherService;
using NotificationService.Api.Services.EmailService;
using NotificationService.Api.Services.MessageConsumerService;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// services
services.AddSingleton<IEmailService, EmailService>();
services.AddSingleton<IEmailPublisher, EmailPublisherService>();
services.AddHostedService<MessageConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// endpoint for send notifications
app.MapPost("/api/notifications/email", async (EmailMessage message, IEmailPublisher publisher) =>
{
    publisher.PublishMessage(message);
    return Results.Ok();
});

app.Run();
