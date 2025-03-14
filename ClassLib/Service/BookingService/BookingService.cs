using ClassLib.DTO.Booking;
using ClassLib.DTO.Payment;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ClassLib.Service
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly UserRepository _userRepository;
        private readonly VaccinesTrackingService _vaccineTrackingService;
        private readonly PaymentMethodRepository _paymentMethodRepository;
        private readonly PaymentRepository _paymentRepository;
        private readonly IWebHostEnvironment _env;
        private readonly EmailService _emailService;
        public BookingService(BookingRepository bookingRepository
                            , UserRepository userRepository
                            , VaccinesTrackingService vaccinesTrackingService
                            , PaymentMethodRepository paymentMethodRepository
                            , PaymentRepository paymentRepository
                            , EmailService emailService
                            , IWebHostEnvironment env)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _vaccineTrackingService = vaccinesTrackingService;
            _paymentMethodRepository = paymentMethodRepository;
            _paymentRepository = paymentRepository;
            _emailService = emailService;
            _env = env;
        }

        public async Task<OrderInfoModel?> AddBooking(AddBooking addBooking)
        {
            Booking booking = ConvertHelpers.convertToBooking(addBooking);

            var checkExistedBookingRepo = await _bookingRepository.GetByBookingID(addBooking.BookingID);
            booking = (await _bookingRepository.AddBooking(checkExistedBookingRepo ?? booking, addBooking.ChildrenIds!, addBooking.vaccineIds!, addBooking.vaccineComboIds!))!;
            AddVaccinesTrackingRequest addVaccinesTrackingRequest = ConvertHelpers.convertToVaccinesTrackingRequest(addBooking);

            // Soft delete the existed vaccines tracking if staff want to change vaccine and vaccineCombo for user
            await _vaccineTrackingService.SoftDeleteByBookingId(booking.Id);

            // Add vaccine to vaccine tracking
            if (!addBooking.vaccineIds.IsNullOrEmpty())
                await _vaccineTrackingService.AddVaccinesToVaccinesTrackingAsync(addVaccinesTrackingRequest, addBooking.vaccineIds!, addBooking.ChildrenIds!, booking!.Id);

            // Add combos to vaccine tracking
            if (!addBooking.vaccineComboIds.IsNullOrEmpty())
                await _vaccineTrackingService.AddVaccinesComboToVaccinesTrackingAsync(addVaccinesTrackingRequest, addBooking.vaccineComboIds!, addBooking.ChildrenIds!, booking!.Id);

            var user = await _userRepository.getUserByIdAsync(addBooking.ParentId);

            return ConvertHelpers.convertToOrderInfoModel(booking!, user!, addBooking);
        }
        public async Task<Booking?> UpdateBookingStatus(string bookingId, string msg)
        {
            var booking = await _bookingRepository.UpdateBooking(bookingId, msg);
            if (msg == BookingEnum.Success.ToString())
            {
                string templatePath = Path.Combine(_env.WebRootPath, "templates", "bookingSuccessTemplate.html");
                var parent = await _bookingRepository.GetByBookingID(int.Parse(bookingId));

                Dictionary<string, string> newDictonary = new Dictionary<string, string>(){
                    { "bookingId" , bookingId},
                    { "dateTimeArrived" , booking!.ArrivedAt.ToString()},
                    { "userName" ,parent!.Parent.Name}
                };
                await _emailService.sendEmailService(parent.Parent.Gmail, "Booking Successful", templatePath, newDictonary);
            }
            if (msg == BookingEnum.Refund.ToString())
            {
                string templatePath = Path.Combine(_env.WebRootPath, "templates", "refundSuccessTemplate.html");
                var parent = await _bookingRepository.GetByBookingID(int.Parse(bookingId));

                Dictionary<string, string> newDictonary = new Dictionary<string, string>(){
                    { "bookingId" , bookingId},
                    { "amount" , ((await _paymentRepository.GetByBookingIDAsync(int.Parse(bookingId)))!.TotalPrice*-1).ToString()},
                    { "userName" ,parent!.Parent.Name}
                };
                await _emailService.sendEmailService(parent.Parent.Gmail, "Refund Successful", templatePath, newDictonary);
            }


            return booking;
        }
        public async Task<List<BookingResponse>?> GetBookingByUserAsync(int id)
        {
            List<BookingResponse> bookingResponses = ConvertHelpers.ConvertBookingResponse((await _bookingRepository.GetAllBookingByUserId(id))!);
            foreach (var item in bookingResponses)
            {
                var payment = (await _paymentRepository.GetByBookingIDAsync(item.ID))!;
                string paymentMethod = "Does not purchase yet";
                decimal amount = 0;
                foreach (var vaccine in item.VaccineList!)
                {
                    amount += vaccine.Price;
                }
                foreach (var combo in item.ComboList!)
                {
                    amount += combo.finalPrice;
                }
                if (payment != null)
                {
                    paymentMethod = (await _paymentMethodRepository.getPaymentMethodById(payment.PaymentMethod))!.Name;
                }
                item.Amount = amount * item.ChildrenList!.Count();
                item.paymentName = paymentMethod;
            }

            return bookingResponses;
        }
        public async Task<List<BookingResponesStaff>?> GetBookingByUserAsyncStaff(int id)
        {
            List<BookingResponesStaff> bookingResponses = ConvertHelpers.ConvertBookingResponseStaff((await _bookingRepository.GetAllBookingByUserIdStaff(id))!);
            foreach (var item in bookingResponses)
            {
                decimal amount = 0;
                foreach (var vaccine in item.VaccineList!)
                {
                    amount += vaccine.Price;
                }
                foreach (var combo in item.ComboList!)
                {
                    amount += combo.finalPrice;
                }
                item.amount = (amount * item.ChildrenList!.Count()).ToString();
            }

            return bookingResponses;
        }
        public async Task<List<BookingResponesStaff>?> GetAllBookingForStaff()
        {
            List<BookingResponesStaff> bookingResponses = ConvertHelpers.ConvertBookingResponseStaff(await _bookingRepository.GetAll());
            foreach (var item in bookingResponses)
            {
                decimal amount = 0;
                foreach (var vaccine in item.VaccineList!)
                {
                    amount += vaccine.Price;
                }
                foreach (var combo in item.ComboList!)
                {
                    amount += combo.finalPrice;
                }
                item.amount= (amount * item.ChildrenList!.Count()).ToString();
            }

            return bookingResponses;
        }
    }
}