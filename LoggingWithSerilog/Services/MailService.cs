using LoggingWithSerilog.Models;
using LoggingWithSerilog.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LoggingWithSerilog.Services;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(MailRequest mailRequest)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
        email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
        email.Subject = mailRequest.Subject;
        var builder = new BodyBuilder();
        if (mailRequest.Attachments != null)
        {
            //byte[] fileBytes;
            foreach (var file in mailRequest.Attachments)
            {
                //if (file.Length > 0)
                //{
                //    using (var ms = new MemoryStream())
                //    {
                //        file.CopyTo(ms);
                //        fileBytes = ms.ToArray();
                //    }
                //    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                //}
                if (file.Length > 0)
                {
                    builder.Attachments.Add(file.FileName, file.OpenReadStream(), ContentType.Parse(file.ContentType));
                }
            }
        }
        builder.HtmlBody = mailRequest.Body;
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        //smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }



    public async Task SendWelcomeEmailAsync(WelcomeRequest request)
    {
        //string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\TemplatesWelcomeTemplate.html";
        string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "WelcomeTemplate.html");

        if (!File.Exists(FilePath))
        {
            throw new FileNotFoundException($"Email template file not found at path: {FilePath}");
        }
        StreamReader str = new StreamReader(FilePath);
        string MailText = str.ReadToEnd();
        str.Close();
        MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        email.From.Add(MailboxAddress.Parse(_mailSettings.Mail));
        email.To.Add(MailboxAddress.Parse(request.ToEmail));
        email.Subject = $"Welcome {request.UserName}";
        var builder = new BodyBuilder();
        builder.HtmlBody = MailText;
        email.Body = builder.ToMessageBody();
        using var smtp = new SmtpClient();
        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        await smtp.SendAsync(email);
        smtp.Disconnect(true);
    }
}