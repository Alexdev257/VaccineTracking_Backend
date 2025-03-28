using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Child;
using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;
using ClassLib.Helpers;
using Microsoft.Identity.Client;

namespace ClassLib.DTO.Booking
{
    public class BookingResponseAdmin
    {
        public string Id { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;
        public string parentName { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public int amount { get; set; } = 0;
        public string status { get; set; } = string.Empty;
        public string AdvisoryDetail { get; set; } = null!;
        public string paymentMethod { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = Helpers.TimeProvider.GetVietnamNow();
        public DateTime arrivedAt { get; set; } = Helpers.TimeProvider.GetVietnamNow();
        public bool IsDeleted { get; set; } = false;
        public List<ChildrenResponeBooking>? ChildrenList { get; set; }
        public List<VaccineResponeBooking>? VaccineList { get; set; }
        public List<ComboResponeBooking>? ComboList { get; set; }
    }
}