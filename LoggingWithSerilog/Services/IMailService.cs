using LoggingWithSerilog.Models;

namespace LoggingWithSerilog.Services;

public interface IMailService
{
    Task SendEmailAsync(MailRequest mailRequest);
    Task SendWelcomeEmailAsync(WelcomeRequest request);
}