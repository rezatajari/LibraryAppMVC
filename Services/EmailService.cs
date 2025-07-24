using LibraryAppMVC.Interfaces;
using LibraryAppMVC.Models;
using LibraryAppMVC.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.Mail;
using System.Net;
using Azure.Core;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace LibraryAppMVC.Services
{
    public class EmailService : IEmailService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(
            UserManager<User> userManager,
            ILogger<EmailService> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<ResultTask<bool>> SendEmail(User user)
        {
            var emailData = await GenerateConfirmationLink(user);
            if (!emailData.Succeeded)
                return ResultTask<bool>.Failure(emailData.ErrorMessage);

            // Get SMTP settings from configuration (app settings.json)
            var smtpHost = _configuration["SMTP:Host"]; // e.g., smtp.gmail.com
            var smtpPort = int.Parse(_configuration["SMTP:Port"] ?? string.Empty); // e.g., 587
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
                From = new MailAddress(smtpUser ?? string.Empty, "Library confirmation your email"), // Sender's email
                Subject = emailData.Data.Subject, // Email subject
                Body = emailData.Data.Message, // Email body
                IsBodyHtml = true // Allow HTML in the email content
            };

            mailMessage.To.Add(emailData.Data.Email); // Add the recipient

            // Send the email
            await smtpClient.SendMailAsync(mailMessage);

            return ResultTask<bool>.Success(true);
        }

        private async Task<ResultTask<EmailModel>> GenerateConfirmationLink(User user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/');
                var confirmationLink = $"{baseUrl}/Account/ConfirmEmail?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

                const string subject = "Confirm your email";
                var message = $"Please confirm your email by clicking the following link: <a href='{confirmationLink}'>Confirm Email</a>";
                var emailModel = new EmailModel
                {
                    Email = user.Email,
                    Subject = subject,
                    Message = message,
                };

                return ResultTask<EmailModel>.Success(data: emailModel);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to {Email}", user.Email);
                return ResultTask<EmailModel>.Failure("Failed to send email confirmation.");
            }
        }
    }
}

