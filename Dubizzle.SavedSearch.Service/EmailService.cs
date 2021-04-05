using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Service
{
    public class EmailService : INotificationService<EmailMessageDto>
    {
        private readonly string _apiKey;
        private readonly string _fromName;
        private readonly string _from;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var sendGrid = configuration.GetSection("SendGrid");

            _apiKey = sendGrid["ApiKey"];
            _from = sendGrid["FromAddress"];
            _fromName = sendGrid["FromName"];
        }
        public async Task SendNotificationAsync(EmailMessageDto message)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_from, _fromName);
            var subject = message.Subject;
            var to = new EmailAddress(message.Recepients.First(), message.Recepients.First());
            var plainTextContent = message.Body;
            var htmlContent = message.Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}