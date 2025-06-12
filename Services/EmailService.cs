using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

public class EmailService
{
    private readonly string smtpServer = "smtp.gmail.com"; // SMTP sunucun
    private readonly int smtpPort = 587;
    private readonly string smtpUser = "your-email@gmail.com";
    private readonly string smtpPass = "your-email-password"; // Güvenli şekilde saklaman önerilir

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Your Name", smtpUser));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(smtpServer, smtpPort, false);
            await client.AuthenticateAsync(smtpUser, smtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
