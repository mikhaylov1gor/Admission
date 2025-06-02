using NotificationService.Api.Models;

namespace NotificationService.Api.Services.EmailService;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
}
