using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using PortfolioBackend.Api.Models;

namespace PortfolioBackend.Api.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? "";
        public int SmtpPort { get; set; } = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        public string SenderEmail { get; set; } = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL") ?? "";
        public string SenderName { get; set; } = Environment.GetEnvironmentVariable("SMTP_SENDER_NAME") ?? "";
        public string Password { get; set; } = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
        public string RecipientEmail { get; set; } = Environment.GetEnvironmentVariable("SMTP_RECIPIENT") ?? "";
    }



    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendContactFormEmailAsync(ContactRequest request)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            email.To.Add(new MailboxAddress("Siddh", _emailSettings.RecipientEmail));
            email.Subject = $"NEW PORTFOLIO CONTACT: {request.Subject}";

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <h2>New Contact Message from Portfolio</h2>
                        <p><strong>Name:</strong> {request.Name}</p>
                        <p><strong>Email:</strong> <a href='mailto:{request.Email}'>{request.Email}</a></p>
                        <p><strong>Subject:</strong> {request.Subject}</p>
                        <hr>
                        <p><strong>Message:</strong></p>
                        <p style='border: 1px solid #ccc; padding: 15px; background-color: #f9f9f9;'>{request.Message}</p>
                        <hr>
                        <p>-- End of Message --</p>
                    </body>
                    </html>"
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
