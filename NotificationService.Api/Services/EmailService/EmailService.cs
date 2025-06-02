using MailKit.Security;
using MimeKit;
using NotificationService.Api.Models;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace NotificationService.Api.Services.EmailService;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;

        var builder = new BodyBuilder();
        if (message.IsHtml)
        {
            builder.HtmlBody = message.Body;
        }
        else
        {
            builder.TextBody = message.Body;
        }

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _configuration["Email:SmtpServer"],
            int.Parse(_configuration["Email:Port"] ?? "1025"),
            SecureSocketOptions.None
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
} 