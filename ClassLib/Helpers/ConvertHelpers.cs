using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Booking;
using ClassLib.DTO.Payment;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Models;

namespace ClassLib.Helpers
{
    public class ConvertHelpers
    {
        public static VaccinesTracking convertToVaccinesTrackingModel(AddVaccinesTrackingRequest request, int childID, Vaccine vaccines, VaccinesTracking previousVaccination)
        {
            return new VaccinesTracking
            {
                VaccineId = vaccines.Id,
                UserId = request.UserId,
                ChildId = childID,
                VaccinationDate = (previousVaccination == null) ? request.VaccinationDate : null,
                Status = "Waiting",
                AdministeredBy = request.AdministeredBy,
                MinimumIntervalDate = (previousVaccination == null) ? request.VaccinationDate!.Value.AddDays(2) : previousVaccination.VaccinationDate!.Value.AddDays(vaccines.MinimumIntervalDate!.Value),
                MaximumIntervalDate = (previousVaccination == null) ? request.VaccinationDate!.Value.AddDays(7) : previousVaccination.VaccinationDate!.Value.AddDays(vaccines.MaximumIntervalDate!.Value),
                Reaction = "Nothing",
                PreviousVaccination = (previousVaccination == null) ? 0 : previousVaccination.Id
            };
        }

        public static VaccinesTrackingResponse convertToVaccinesTrackingResponse(VaccinesTracking vt)
        {
            return new VaccinesTrackingResponse
            {
                VaccineName = vt.Vaccine.Name,
                UserName = vt.User.Name,
                ChildName = vt.Child.Name,
                MinimumIntervalDate = vt.MinimumIntervalDate,
                VaccinationDate = vt.VaccinationDate,
                MaximumIntervalDate = vt.MaximumIntervalDate,
                Status = vt.Status,
                AdministeredByDoctorName = vt.User.Name,
                Reaction = vt.Reaction
            };
        }

        public static AddVaccinesTrackingRequest convertToVaccinesTrackingRequest(AddBooking addBooking)
        {
            return new AddVaccinesTrackingRequest
            {
                UserId = addBooking.ParentId,
                VaccinationDate = addBooking.ArrivedAt,
                AdministeredBy = 0
            };
        }

        public static Booking convertToBooking(AddBooking addBooking)
        {
            return new Booking
            {
                ParentId = addBooking.ParentId,
                AdvisoryDetails = addBooking.AdvisoryDetail,
                ArrivedAt = addBooking.ArrivedAt,
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };
        }

        public static OrderInfoModel convertToOrderInfoModel(Booking booking, User user, AddBooking addBooking)
        {
            return new OrderInfoModel
            {
                GuestName = user.Name!,
                GuestEmail = user.Gmail!,
                GuestPhone = user.PhoneNumber!,
                BookingID = booking.Id.ToString(),
                OrderId = booking.Id.ToString(),
                OrderDescription = booking.AdvisoryDetails,
                Amount = addBooking.TotalPrice
            };
        }
    }
}