using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Repositories;
using ClassLib.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

namespace ClassLib.BackGroundServices
{
    public class VaccineTrackingReminderJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<VaccineTrackingReminderJob> _logger;
        

        public VaccineTrackingReminderJob(IServiceProvider serviceProvider, ILogger<VaccineTrackingReminderJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();
                        var vaccineTrackingRepository = scope.ServiceProvider.GetRequiredService<VaccinesTrackingRepository>();
                        var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();
                        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                        await SendUpcomingVaccineReminder(userRepository, vaccineTrackingRepository, emailService, env);
                        await SendDeadlineVaccineReminder(userRepository, vaccineTrackingRepository, emailService, env);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error in VaccineTrackingReminderTask: {e.Message}");
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Chạy mỗi 24 giờ
            }
                
        }

        private async Task<bool> SendUpcomingVaccineReminder(UserRepository userRepository, VaccinesTrackingRepository vaccinesTrackingRepository, EmailService emailService, IWebHostEnvironment env)
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            var upComingVaccinations = await vaccinesTrackingRepository.GetUpComingVaccinations(today);
            var allMailSent = true;

            foreach (var upComing in upComingVaccinations)
            {
                var user = await userRepository.getUserByIdAsync(upComing.UserId);
                if (user == null || string.IsNullOrWhiteSpace(user.Gmail))
                {
                    continue;
                }

                string toEmail = user.Gmail;
                string subject = $"Vaccination Reminder For {user.Name}";
                string templatePath = Path.Combine(env.WebRootPath, "templates", "vaccinationsReminder.html");
                var placeholders = new Dictionary<string, string>()
                {
                    {"UserName", user.Name },
                    {"VaccineName", upComing.Vaccine.Name},
                    {"VaccinationDate", upComing.MinimumIntervalDate?.ToString("dd/MM/yyyy") ?? "N/A"},
                    {"ChildName", upComing.Child.Name },
                    { "SupportEmail", "healthbluecaresystem@gmail.com" }
                };

                var smtpRetryPolicy = Policy
                    .Handle<SmtpException>(e => e.StatusCode == SmtpStatusCode.ServiceNotAvailable) // diconnect
                    .Or<TaskCanceledException>() // timeout mail sending
                    .Or<HttpRequestException>()
                    .WaitAndRetryAsync(5, retry => TimeSpan.FromSeconds(5 * retry),
                    (e, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"SMTP error sending email to {toEmail}. Retrying ({retryCount}/5)... Error: {e.Message}");
                    });

                var inboxFullRetryPolicy = Policy
                    .Handle<SmtpException>(e => e.Message.Contains("Mailbox full"))
                    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(5 * retry),
                    (e, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Email to {toEmail} failed due to full inbox. Retrying ({retryCount}/3)... Error: {e.Message}");
                    });

                var addressNotFoundRetryPolicy = Policy
                    .Handle<SmtpException>(e => e.Message.Contains("Address not found"))
                    .WaitAndRetryAsync(2, retry => TimeSpan.FromSeconds(3 * retry), 
                    (e, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Email to {toEmail} failed due to invalid address. Retrying ({retryCount}/2)... Error: {e.Message}");
                    });

                //combine all exception
                var combinedPolicy = Policy.WrapAsync(smtpRetryPolicy, inboxFullRetryPolicy, addressNotFoundRetryPolicy);


                bool isSent = await combinedPolicy.ExecuteAsync(
                    async () => await emailService.sendEmailService(toEmail, subject, templatePath, placeholders)
                );
                if (!isSent)
                {
                    allMailSent = false;
                }
            }
            return allMailSent;
        }

        private async Task<bool> SendDeadlineVaccineReminder(UserRepository userRepository, VaccinesTrackingRepository vaccinesTrackingRepository, EmailService emailService, IWebHostEnvironment env)
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            var deadlineVaccinations = await vaccinesTrackingRepository.GetDeadlineVaccinations(today);
            var allMailSent = true;

            foreach (var deadline in deadlineVaccinations)
            {
                var user = await userRepository.getUserByIdAsync(deadline.UserId);
                if (user == null || string.IsNullOrWhiteSpace(user.Gmail))
                {
                    continue;
                }

                string toEmail = user.Gmail;
                string subject = $"Vaccination Reminder For {user.Name}";
                string templatePath = Path.Combine(env.WebRootPath, "templates", "vaccinationsDeadlineReminder.html");
                var placeholders = new Dictionary<string, string>()
                {
                    {"UserName", user.Name },
                    {"VaccineName", deadline.Vaccine.Name},
                    {"VaccinationDate", deadline.MinimumIntervalDate?.ToString("dd/MM/yyyy") ?? "N/A"},
                    {"ChildName", deadline.Child.Name },
                    { "SupportEmail", "healthbluecaresystem@gmail.com" }
                };

                var smtpRetryPolicy = Policy
                    .Handle<SmtpException>(e => e.StatusCode == SmtpStatusCode.ServiceNotAvailable) // diconnect
                    .Or<TaskCanceledException>() // timeout mail sending
                    .Or<HttpRequestException>()
                    .WaitAndRetryAsync(5, retry => TimeSpan.FromSeconds(5 * retry),
                    (e, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"SMTP error sending email to {toEmail}. Retrying ({retryCount}/5)... Error: {e.Message}");
                    });

                var inboxFullRetryPolicy = Policy
                    .Handle<SmtpException>(e => e.Message.Contains("Mailbox full"))
                    .WaitAndRetryAsync(3, retry => TimeSpan.FromSeconds(5 * retry),
                    (e, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Email to {toEmail} failed due to full inbox. Retrying ({retryCount}/3)... Error: {e.Message}");
                    });

                var addressNotFoundRetryPolicy = Policy
                    .Handle<SmtpException>(e => e.Message.Contains("Address not found"))
                    .WaitAndRetryAsync(2, retry => TimeSpan.FromSeconds(3 * retry),
                    (e, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Email to {toEmail} failed due to invalid address. Retrying ({retryCount}/2)... Error: {e.Message}");
                    });

                //combine all exception
                var combinedPolicy = Policy.WrapAsync(smtpRetryPolicy, inboxFullRetryPolicy, addressNotFoundRetryPolicy);


                bool isSent = await combinedPolicy.ExecuteAsync(
                    async () => await emailService.sendEmailService(toEmail, subject, templatePath, placeholders)
                );
                if (!isSent)
                {
                    allMailSent = false;
                }
            }
            return allMailSent;
        }
    }
}
