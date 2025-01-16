namespace LoggingWithSerilog.Models;

public class MailRequest
{
    public string? ToEmail { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public List<IFormFile>? Attachments { get; set; }
    public Dictionary<string, string>? PlaceholderValues { get; set; }
    public string? Template { get; set; }
}