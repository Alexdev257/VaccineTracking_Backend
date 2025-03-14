using ClassLib.DTO.Booking;
using ClassLib.DTO.Child;
using ClassLib.DTO.Payment;
using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Enum;
using ClassLib.Models;
using PayPal.v1.Orders;

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
            if (vt == null)
            {
                throw new ArgumentNullException(nameof(vt), "VaccinesTracking object is null.");
            }
            if (vt.Vaccine == null)
            {
                throw new NullReferenceException($"Vaccine is null for TrackingID: {vt.Id}");
            }
            if (vt.User == null)
            {
                throw new NullReferenceException($"User is null for TrackingID: {vt.Id}");
            }

            return new VaccinesTrackingResponse
            {
                TrackingID = vt.Id,
                VaccineName = vt.Vaccine.Name,
                UserName = vt.User.Name,
                ChildId = vt.ChildId,
                MinimumIntervalDate = vt.MinimumIntervalDate.HasValue ? vt.MinimumIntervalDate.ToString() : "",
                VaccinationDate = vt.VaccinationDate.HasValue ? vt.VaccinationDate.ToString() : "",
                MaximumIntervalDate = vt.MaximumIntervalDate.HasValue ? vt.MaximumIntervalDate.ToString() : "",//toan tu ba ngoi
                PreviousVaccination = vt.PreviousVaccination.HasValue ? (int)vt.PreviousVaccination.Value : 0,
                Status = vt.Status,
                AdministeredByDoctorName = "Not Vaccination Yet",
                Reaction = vt.Reaction,
                VaccineID = vt.VaccineId,
                BookingId = vt.BookingId
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
                IsDeleted = false,
                Id = addBooking.BookingID
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

        public static OrderInfoModel RepurchaseBookingtoOrderInfoModel(Booking booking, User user, int amount)
        {
            return new OrderInfoModel
            {
                GuestName = user.Id + " " + user.Name!,
                GuestEmail = user.Gmail!,
                GuestPhone = user.PhoneNumber!,
                BookingID = booking.Id.ToString(),
                OrderId = booking.Id.ToString(),
                OrderDescription = booking.AdvisoryDetails,
                Amount = amount
            };
        }

        public static List<VaccineResponeBooking> ConvertListVaccines(List<Vaccine> listVaccines)
        {
            List<VaccineResponeBooking> list = new List<VaccineResponeBooking>();
            foreach (var item in listVaccines)
            {
                VaccineResponeBooking vrb = new VaccineResponeBooking()
                {
                    ID = item.Id,
                    Name = item.Name,
                    Price = item.Price
                };

                list.Add(vrb);
            }
            return list;
        }

        public static List<PaymentResponseStaff> ConvertPaymentResponseStaff(List<Payment> listPayments)
        {
            List<PaymentResponseStaff> list = new List<PaymentResponseStaff>();

            foreach (var item in listPayments)
            {
                PaymentResponseStaff prs = new PaymentResponseStaff()
                {
                    paymentId = item.PaymentId,
                    paymentName = item.PaymentMethodNavigation.Name,
                    amount = item.TotalPrice
                };

                list.Add(prs);
            }

            return list;
        }

        public static List<ComboResponeBooking> ConvertListCombos(List<VaccinesCombo> listCombos)
        {
            List<ComboResponeBooking> list = new List<ComboResponeBooking>();
            foreach (var item in listCombos)
            {
                ComboResponeBooking crb = new ComboResponeBooking()
                {
                    ID = item.Id,
                    Name = item.ComboName,
                    Discount = item.Discount,
                    totalPrice = (int)item.TotalPrice,
                    finalPrice = (int)item.FinalPrice,
                    vaccineResponeBooking = ConvertListVaccines((List<Vaccine>)item.Vaccines),
                };
                list.Add(crb);
            }
            return list;
        }

        public static List<ChildrenResponeBooking> ConvertListChildren(List<Child> listChildrens)
        {
            List<ChildrenResponeBooking> list = new List<ChildrenResponeBooking>();
            foreach (var item in listChildrens)
            {
                ChildrenResponeBooking crb = new ChildrenResponeBooking()
                {
                    ChildId = item.Id,
                    Name = item.Name,
                    Gender = item.Gender
                };
                list.Add(crb);
            }
            return list;
        }

        public static List<BookingResponse> ConvertBookingResponse(List<Booking> listBookings)
        {
            List<BookingResponse> list = new List<BookingResponse>();
            foreach (var item in listBookings)
            {
                BookingResponse br = new BookingResponse()
                {
                    ID = item.Id,
                    AdvisoryDetail = item.AdvisoryDetails,
                    ArrivedAt = item.ArrivedAt.ToString("HH:mm:ss dd-MM-yyyy"),
                    ChildrenList = ConvertListChildren((List<Child>)item.Children),
                    VaccineList = ConvertListVaccines((List<Vaccine>)item.Vaccines),
                    ComboList = ConvertListCombos((List<VaccinesCombo>)item.Combos),
                    Status = item.Status,
                };
                list.Add(br);
            }
            return list;
        }
        public static List<BookingResponesStaff> ConvertBookingResponseStaff(List<Booking> bookings){
            List<BookingResponesStaff> list = new List<BookingResponesStaff>();

            foreach(var item in bookings){
                BookingResponesStaff brs = new BookingResponesStaff(){
                    Id = item.Id.ToString(),
                    parentName = item.Parent.Name,
                    phoneNumber = item.Parent.PhoneNumber,
                    status = item.Status,
                    paymentMethod = item.Payments.LastOrDefault()!.PaymentMethodNavigation.Name,
                    ChildrenList = ConvertListChildren((List<Child>)item.Children),
                    VaccineList = ConvertListVaccines((List<Vaccine>)item.Vaccines),
                    ComboList = ConvertListCombos((List<VaccinesCombo>)item.Combos)
                };

                list.Add(brs);
            }
            return list;
        }

        public static UpdateVaccineTracking ConvertToUpdateVaccineTracking(UpdateVaccineTrackingUser updateVaccineTrackingUser)
        {
            return new UpdateVaccineTracking()
            {
                Reaction = updateVaccineTrackingUser.Reaction,
                Status = ((updateVaccineTrackingUser.isCancel == true) ? "cancel" : null)!,
                Reschedule = updateVaccineTrackingUser?.Reschedule
            };
        }
    }
}