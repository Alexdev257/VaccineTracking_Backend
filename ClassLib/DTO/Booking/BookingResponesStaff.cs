using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Child;
using ClassLib.DTO.Payment;
using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;
using ClassLib.Helpers;
using TimeProvider = ClassLib.Helpers.TimeProvider;

namespace ClassLib.DTO.Booking
{
    public class BookingResponesStaff
    {
        public string Id { get; set; } = string.Empty;

        public string ParentId {get;set;} = string.Empty;
        public string parentName { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public int amount { get; set; } = 0;
        public string status { get; set; } = string.Empty;
        public string AdvisoryDetail { get; set; } = null!;
        public string paymentMethod { get; set; } = string.Empty;
        public DateTime createdAt { get; set; } = TimeProvider.GetVietnamNow();
        public DateTime arrivedAt { get; set; } = TimeProvider.GetVietnamNow();
        public List<ChildrenResponeBooking>? ChildrenList { get; set; }
        public List<VaccineResponeBooking>? VaccineList { get; set; }
        public List<ComboResponeBooking>? ComboList { get; set; }
    }
}