using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Repositories;
using ClassLib.SignalRHub;
using Microsoft.AspNetCore.SignalR;

namespace ClassLib.Service
{
    public class NotificationService
    {
        private readonly VaccinesTrackingRepository _vaccinesTrackingRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(VaccinesTrackingRepository vaccinesTrackingRepository, IHubContext<NotificationHub> hubContext)
        {
            _vaccinesTrackingRepository = vaccinesTrackingRepository;
            _hubContext = hubContext;
        }

        public async Task SendUpComingVaccinationNotifications()
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            var upcomingVaccinations = await _vaccinesTrackingRepository.GetUpComingVaccinations(today);

            foreach (var tracking in upcomingVaccinations)
            {
                var userId = tracking.UserId.ToString();
                var connectionId = NotificationHub.GetConnectionId(userId);

                if (!string.IsNullOrEmpty(connectionId))
                {
                    var data = new
                    {
                        type = "upcoming",
                        user = tracking.User.Name,
                        child = tracking.Child.Name,
                        vaccine = tracking.Vaccine.Name,
                        date = tracking.MinimumIntervalDate,
                    };

                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", data);
                }
            }
        }

        public async Task SendDeadlinevaccinationNotifications()
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            var deadlineVaccinations = await _vaccinesTrackingRepository.GetDeadlineVaccinations(today);

            foreach (var tracking in deadlineVaccinations)
            {
                var userId = tracking.UserId.ToString();
                var connectionId = NotificationHub.GetConnectionId(userId);

                if (!string.IsNullOrEmpty(connectionId))
                {
                    var data = new
                    {
                        type = "deadline",
                        user = tracking.User.Name,
                        child = tracking.Child.Name,
                        vaccine = tracking.Vaccine.Name,
                        date = tracking.MinimumIntervalDate,
                    };
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", data);
                }
            }
        }
    }
}

