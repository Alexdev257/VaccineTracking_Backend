using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Repositories;
using ClassLib.Service;
using Microsoft.AspNetCore.Hosting;

namespace ClassLib.Job
{
    public class VaccineTrackingReminderHangfire
    {
        private readonly UserRepository _userRepository;
        private readonly VaccinesTrackingRepository _vaccinesTrackingRepository;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;


        public VaccineTrackingReminderHangfire(UserRepository userRepository, VaccinesTrackingRepository vaccinesTrackingRepository, EmailService emailService, IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _vaccinesTrackingRepository = vaccinesTrackingRepository;
            _emailService = emailService;
            _env = env;
        }

        public async Task<bool> SendVaccineReminder()
        {
            var today = DateTime.UtcNow;
            var upComingVaccinations = await _vaccinesTrackingRepository.GetUpComingVaccinations(today);
            var allMailSent = true;

            foreach ( var upComing  in upComingVaccinations )
            {
                var user = await _userRepository.getUserByIdAsync(upComing.UserId);
                if( user == null || string.IsNullOrEmpty(user.Gmail))
                {
                    continue;
                }

                string toEmail = user.Gmail;
                string subject = "Vaccinations Reminder For You";
                string templatePath = Path.Combine(_env.WebRootPath, "templates", "vaccinationsReminder.html");
                var placeholders = new Dictionary<string, string>
                {
                    { "UserName", user.Name },
                    { "VaccineName", upComing.Vaccine.Name },
                    { "VaccinationDate", upComing.MinimumIntervalDate?.ToString("dd/MM/yyyy") ?? "N/A" },
                    { "ChildName", upComing.Child.Name },
                    { "SupportEmail", "healthbluecaresystem@gmail.com" }
                };

                bool isSent = await _emailService.sendEmailService(toEmail, subject, templatePath, placeholders);
                if (!isSent)
                {
                    allMailSent = false;
                }
            }
            return allMailSent;
            
        }
    }
}
