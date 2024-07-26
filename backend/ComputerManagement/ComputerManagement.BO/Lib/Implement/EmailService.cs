using ComputerManagement.BO.Lib.Interface;
using ComputerManagement.Common.Configs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Lib.Implement
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EmailService(IOptionsMonitor<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.CurrentValue;
        }

        public EmailConfig GetEmailConfig()
        {
            return _emailConfig;
        }
        public async Task SenEmailAsync(string receiver, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_emailConfig.FromEmail);
            message.Subject = subject;
            message.To.Add(new MailAddress(receiver));
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient(_emailConfig.SmtpClient))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential(_emailConfig.FromEmail, _emailConfig.FromPassword);
                smtpClient.EnableSsl = true; // Enable SSL/TLS

                try
                {
                    await smtpClient.SendMailAsync(message);

                }
                catch (Exception ex)
                {
                    // ghi log
                    throw;
                }
            }
        }
    }
}
