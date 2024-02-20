using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using UI.MVC.Helpers;
using UI.MVC.Interfaces;
using Azure;

namespace UI.MVC.Services
{
    public sealed class SendGridEmail : ISendGridEmail
    {
        private readonly ILogger<SendGridEmail> logger;

        public AuthMessageSenderOptions Options { get; set; }

        public SendGridEmail(IOptions<AuthMessageSenderOptions> opt, ILogger<SendGridEmail> logger)
        {
            Options = opt.Value;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if(String.IsNullOrWhiteSpace(Options.ApiKey))
                throw new InvalidOperationException("INVALID API KEY");

            await Execute(Options.ApiKey, subject, message, toEmail);


        }


        async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            try
            {
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(Options.SenderMailAddress, "Meccu Enterprise");
                var to = new EmailAddress("ozkancamucahit@gmail.com", "Meccu");

                var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
                var response = await client.SendEmailAsync(msg);

                logger.LogInformation(response.IsSuccessStatusCode ? 
                    $"Email to {to} queded successfully!" :
                    $"FAILEDTO SEN EMAIL TO : {to}");
            }
            catch (Exception ex)
            {
                logger.LogInformation("ERROR EXCEPTION SENDING MAIL :" + ex.Message);
            }
        }

    }
}
