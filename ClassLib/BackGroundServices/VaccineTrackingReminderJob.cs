using System.Net.Mail;
using System.Text;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Enum;
using ClassLib.Models;
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
                        var vaccineRepository = scope.ServiceProvider.GetRequiredService<VaccineRepository>();
                        var vaccineComboRepository = scope.ServiceProvider.GetRequiredService<VaccineComboRepository>();
                        var vaccineTrackingService = scope.ServiceProvider.GetRequiredService<VaccinesTrackingService>();

                        await SendUpcomingVaccineReminder(userRepository, vaccineTrackingRepository, emailService, env);
                        await SendDeadlineVaccineReminder(userRepository, vaccineTrackingRepository, emailService, env);
                        await UpdateStatusNearlyExpiredVaccineAndCombo(vaccineRepository,vaccineComboRepository, userRepository, emailService, env);
                        await UpdateStatusExpiredVaccineAndCombo(vaccineRepository, vaccineComboRepository);
                        await UpdateCancelStatusForOverdueTracking(vaccineTrackingRepository, vaccineTrackingService);


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

        private async Task<bool> UpdateStatusNearlyExpiredVaccineAndCombo(VaccineRepository vaccineRepository, VaccineComboRepository vaccineComboRepository, UserRepository userRepository, EmailService emailService, IWebHostEnvironment env)
        {
            var expiredVaccinations = await vaccineRepository.GetNearlyExpiredVaccine();
            var affectedComboIds = new HashSet<int>(); // O(1)
            var allUpdated = true;

            var vaccineListHtml = new StringBuilder();
            foreach (var expired in expiredVaccinations)
            {
                expired.Status = "Nearlyoutstock";
                var isUpdated = await vaccineRepository.UpdateVaccine(expired);
                if (!isUpdated)
                {
                    allUpdated = false;
                }
                else
                {
                    vaccineListHtml.Append($"<li><strong>{expired.Name}</strong> (ID: {expired.Id})</li>");
                    if (expired.VacineCombos != null)
                    {
                        foreach (var combo in expired.VacineCombos)
                        {
                            affectedComboIds.Add(combo.Id);
                        }
                    }
                }
            }

            if (affectedComboIds.Any())
            {
                var allCombos = await vaccineComboRepository.GetAllVaccineComboAdmin();
                var affectedCombos = allCombos.Where(c => affectedComboIds.Contains(c.Id)).ToList();

                foreach (var affectedCombo in affectedCombos)
                {
                    affectedCombo.Status = "Nearlyoutstock";
                    var isUpdated = await vaccineComboRepository.UpdateCombo(affectedCombo);
                    if (!isUpdated)
                    {
                        allUpdated = false;
                    }
                }
            }
            var allUsers = await userRepository.getAll();
            var allAdmin = allUsers.Where(u => u.Role.ToLower() == "admin".ToLower()).ToList();
            foreach (var admin in allAdmin)
            {
                var toEmail = admin.Gmail;
                string subject = $"Vaccines is Out of Stock Reminder For {admin.Name}";
                string templatePath = Path.Combine(env.WebRootPath, "templates", "vaccinesOutStockReminder.html");
                var placeholders = new Dictionary<string, string>()
                {
                    { "UserName", admin.Name },
                    { "VaccineList", vaccineListHtml.ToString() }, // Thêm danh sách vaccine vào email
                    { "SupportEmail", "healthbluecaresystem@example.com" }
                };
                var rs = await emailService.sendEmailService(toEmail, subject, templatePath, placeholders);
                if (!rs)
                {
                    allUpdated = false;
                }
            }
            
            return allUpdated;
        }

        private async Task<bool> UpdateStatusExpiredVaccineAndCombo(VaccineRepository vaccineRepository, VaccineComboRepository vaccineComboRepository)
        {
            var expiredVaccinations = await vaccineRepository.GetExpiredVaccine();
            var affectedComboIds = new HashSet<int>(); // O(1)
            var allUpdated = true;
            foreach (var expired in expiredVaccinations)
            {
                expired.Status = "Outstock";
                var isUpdated = await vaccineRepository.UpdateVaccine(expired);
                if (!isUpdated)
                {
                    allUpdated = false;
                }
                else
                {
                    if (expired.VacineCombos != null)
                    {
                        foreach (var combo in expired.VacineCombos)
                        {
                            affectedComboIds.Add(combo.Id);
                        }
                    }
                }
            }

            if (affectedComboIds.Any())
            {
                var allCombos = await vaccineComboRepository.GetAllVaccineComboAdmin();
                var affectedCombos = allCombos.Where(c => affectedComboIds.Contains(c.Id)).ToList();

                foreach (var affectedCombo in affectedCombos)
                {
                    affectedCombo.Status = "Outstock";
                    var isUpdated = await vaccineComboRepository.UpdateCombo(affectedCombo);
                    if (!isUpdated)
                    {
                        allUpdated = false;
                    }
                }
            }
            return allUpdated;
        }

        public async Task<bool> UpdateCancelStatusForOverdueTracking(VaccinesTrackingRepository vaccinesTrackingRepository, VaccinesTrackingService vaccinesTrackingService)
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            var overdueTracking = await vaccinesTrackingRepository.GetOverdueTracking(today);
            bool allOk = false;
            foreach (var combo in overdueTracking)
            {
                UpdateVaccineTracking update = new UpdateVaccineTracking
                {
                    Status = VaccinesTrackingEnum.Cancel.ToString(),
                    Reaction = combo.Reaction,
                    AdministeredBy = combo.AdministeredBy,
                    Reschedule = null,
                };

                var rs = await vaccinesTrackingService.UpdateVaccinesTrackingAsync(combo.Id, update);

                if (rs)
                {
                    allOk = true;
                }

            }
            return allOk;
        }
    }
}
