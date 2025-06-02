using NotificationService.Api.Models;

namespace NotificationService.Api.Services.EmailPublisherService;

public interface IEmailPublisher
{
    void PublishMessage(EmailMessage message);
}