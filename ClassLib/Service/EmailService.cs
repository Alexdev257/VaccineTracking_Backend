using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Repositories;

namespace ClassLib.Service
{
    public class EmailService
    {
        private readonly EmailRepository _emailRepository;

        public EmailService(EmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<bool> sendEmailService(string toEmail, string subject, string templatePath, Dictionary<string, string> placeholders)
        {
            string body = await File.ReadAllTextAsync(templatePath);

            // replace placeholders in the email template into real values
            foreach (var placeholder in placeholders)
            {
                body = body.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            return await _emailRepository.SendEmailAsync(toEmail, subject, body);
        }
    }
}
