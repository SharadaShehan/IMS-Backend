using SendGrid;
using SendGrid.Helpers.Mail;
using System.Diagnostics;

namespace IMS.Infrastructure.Services
{
    public class EmailService
    {
        private SendGridClient Client { get; set; }
        private EmailAddress FromEmail { get; set; }
        public interface IEmailService
        {
            public Task<Boolean> SendEmail(string to, string subject, string plainTextContent, string htmlContent);
        }

        public EmailService(EmailServiceContextOptions options)
        {
            Client = new SendGridClient(options.APIKey);
            FromEmail = new EmailAddress(options.SenderEmail, "IMS");
        }

        public async Task<Boolean> SendEmail(string receiverEmail, string subject, string plainTextContent, string htmlContent)
        {
            try
            {
                EmailAddress ToEmail = new EmailAddress(receiverEmail);
                var msg = MailHelper.CreateSingleEmail(FromEmail, ToEmail, subject, plainTextContent, htmlContent);
                var response = await Client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode) return true;
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }

    public class EmailServiceContextOptions
    {
        public string APIKey { get; set; }
        public string SenderEmail { get; set; }
    }
}