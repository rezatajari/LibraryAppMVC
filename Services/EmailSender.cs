using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace YourProject.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Get SMTP settings from configuration (appsettings.json)
            var smtpHost = _configuration["SMTP:Host"]; // e.g., smtp.gmail.com
            var smtpPort = int.Parse(_configuration["SMTP:Port"]); // e.g., 587
            var smtpUser = _configuration["SMTP:Username"]; // Your email
            var smtpPass = _configuration["SMTP:Password"]; // Your email password

            // Set up the SMTP client
            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true // Use SSL for secure connection
            };

            // Create the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser, "Library System"), // Sender's email
                Subject = subject, // Email subject
                Body = message, // Email body
                IsBodyHtml = true // Allow HTML in the email content
            };

            mailMessage.To.Add(email); // Add the recipient

            // Send the email
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
