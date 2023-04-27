using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Tarteeb.Api.Models.Foundations.Users;

namespace Tarteeb.Api.UtilityService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void SendEmail(EmailModel emailModel)
        {
            var emailMessage = new MimeMessage();
            var from = _config["EmailSettings:From"];
            emailMessage.From.Add(new MailboxAddress("Javlon Rahmatullayev", from));
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = string.Format(emailModel.Content)
            };

            using(var client= new SmtpClient())
            {
                try
                {
                    client.Connect(_config["EmailSettings:SmtpServer"],465,true);
                    client.Authenticate(_config["EmailSettings:Form"], _config["EmailSettings:Password"]);
                    client.Send(emailMessage);
                }
                catch (System.Exception)
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
