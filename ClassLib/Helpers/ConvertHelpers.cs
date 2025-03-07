using ClassLib.DTO.Booking;
using ClassLib.DTO.Payment;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Enum;
using ClassLib.Models;

namespace ClassLib.Helpers
{
    public class ConvertHelpers
    {
        public static VaccinesTracking convertToVaccinesTrackingModel(AddVaccinesTrackingRequest request, int childID, Vaccine vaccines, VaccinesTracking previousVaccination, int bookingID)
        {
            return new VaccinesTracking
            {
                VaccineId = vaccines.Id,
                UserId = request.UserId,
                ChildId = childID,
                VaccinationDate = (previousVaccination == null) ? request.VaccinationDate : null,
                Status = (previousVaccination == null) ? ((VaccinesTrackingEnum)VaccinesTrackingEnum.Schedule).ToString() : ((VaccinesTrackingEnum)VaccinesTrackingEnum.Waiting).ToString(),
                AdministeredBy = request.AdministeredBy,
                MinimumIntervalDate = (previousVaccination == null)
                                        ? (request.VaccinationDate ?? TimeProvider.GetVietnamNow()).AddDays(2)  // Use DateTime.Now as fallback
                                        : null,
                MaximumIntervalDate = (previousVaccination == null)
                                        ? (request.VaccinationDate ?? TimeProvider.GetVietnamNow()).AddDays(7)  // Use DateTime.Now as fallback
                                        : null,
                Reaction = "Nothing",
                PreviousVaccination = (previousVaccination == null) ? 0 : previousVaccination.Id,
                BookingId = bookingID
            };
        }

        public static VaccinesTrackingResponse convertToVaccinesTrackingResponse(VaccinesTracking vt)
        {
            return new VaccinesTrackingResponse
            {
                TrackingID = vt.Id,
                VaccineName = vt.Vaccine.Name,
                UserName = vt.User.Name,
                ChildName = vt.Child.Name,
                MinimumIntervalDate = vt.MinimumIntervalDate,
                VaccinationDate = vt.VaccinationDate,
                MaximumIntervalDate = vt.MaximumIntervalDate,
                PreviousVaccination = (int)vt.PreviousVaccination!,
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
                CreatedAt = TimeProvider.GetVietnamNow(),
                Status = ((BookingEnum)BookingEnum.Pending).ToString(),
                IsDeleted = false
            };
        }

        public static RefundModel convertToRefundModel(Payment payment, double amount, int refundType)
        {
            return new RefundModel
            {
                amount = amount,
                payerID = payment.PayerId,
                paymentDate = payment.PaymentDate,
                paymentID = payment.PaymentId,
                currency = payment.Currency,
                trancasionID = payment.TransactionId,
                RefundType = refundType
            };
        }

        public static OrderInfoModel convertToOrderInfoModel(Booking booking, User user, AddBooking addBooking)
        {
            return new OrderInfoModel
            {

                GuestName = user.Id + " " + user.Name!,
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