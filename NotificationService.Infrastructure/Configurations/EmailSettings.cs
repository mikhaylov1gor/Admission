namespace NotificationService.Application.Configurations;

public class EmailSettings
{
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
}