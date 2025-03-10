using ClassLib.Repositories;
using ClassLib.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

                        await SendVaccineReminder(userRepository, vaccineTrackingRepository, emailService, env);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Error in VaccineTrackingReminderTask: {e.Message}");
                }
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // Chạy mỗi 24 giờ
            }
                
        }

        private async Task<bool> SendVaccineReminder(UserRepository userRepository, VaccinesTrackingRepository vaccinesTrackingRepository, EmailService emailService, IWebHostEnvironment env)
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

                bool isSent = await emailService.sendEmailService(toEmail, subject, templatePath, placeholders);
                if(!isSent)
                {
                    allMailSent = false;
                }
            }
            return allMailSent;
        }
    }
}
