namespace NotificationService.Infrastructure.Services.EmailSender;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}