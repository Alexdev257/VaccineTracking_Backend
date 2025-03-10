using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ClassLib.Repositories
{
    public class EmailRepository
    {
        private readonly IConfiguration _configuration;

        public EmailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            string smtpServer = emailSettings["SmtpServer"]!;
            int smtpPort = int.Parse(emailSettings["SmtpPort"]!);
            string senderEmail = emailSettings["SenderEmail"]!;
            string senderPassword = emailSettings["SenderPassword"]!;
            string senderName = emailSettings["SenderName"]!;

            using(var client = new SmtpClient(smtpServer))
            {
                client.Port = smtpPort;
                //client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                client.EnableSsl = true;
                var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);
                try
                {
                    await client.SendMailAsync(message);
                    return true;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
            
    }
}
