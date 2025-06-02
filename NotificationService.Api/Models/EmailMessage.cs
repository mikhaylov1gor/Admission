namespace NotificationService.Api.Models;

public class EmailMessage
{
    public required string To { get; set; } 
    public required string Subject { get; set; }
    public required string Body { get; set; } 
    public required bool IsHtml { get; set; }
} 