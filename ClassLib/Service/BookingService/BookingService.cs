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

        /// <summary>
        /// Update existed Booking
        /// </summary>
        /// <param name="updateBooking"></param>
        /// <returns>Status</returns>
        public async Task<string> UpdateBookingDetails(UpdateBooking updateBooking)
        {
            var booking = await _bookingRepository.GetByBookingID(updateBooking.BookingId);
            if (booking == null) return "Don't exist booking";
            if (booking.Status == BookingEnum.Success.ToString()) return "Booking is already success";
            var listChild = ConvertHelpers.ConvertChildrenToListInt(booking.Children.ToList());
            booking = (await _bookingRepository.AddBooking(booking, listChild, updateBooking.VaccinesList!, updateBooking.VaccinesCombo!))!;
            return "Success";
        }
        /// <summary>
        /// Add booking to DB include BookingChild, BookingVaccine, BookingCombo
        /// </summary>
        /// <param name="addBooking"></param>
        /// <returns>Order Payment Object</returns>
        public async Task<OrderInfoModel?> AddBooking(AddBooking addBooking)
        {
            Booking booking = ConvertHelpers.convertToBooking(addBooking);
            var checkExistedBookingRepo = await _bookingRepository.GetByBookingID(addBooking.BookingID);
            booking = (await _bookingRepository.AddBooking(checkExistedBookingRepo ?? booking, addBooking.ChildrenIds!, addBooking.vaccineIds!, addBooking.vaccineComboIds!))!;
            var user = await _userRepository.getUserByIdAsync(addBooking.ParentId);

            return ConvertHelpers.convertToOrderInfoModel(booking!, user!, addBooking);
        }
        /// <summary>
        /// Update booking status
        /// </summary>
        /// <param name="bookingId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<Booking?> UpdateBookingStatus(string bookingId, string msg)
        {
            var booking = await _bookingRepository.UpdateBooking(bookingId, msg);
            if (msg.ToLower() == BookingEnum.Success.ToString().ToLower())
            {
                booking = await _bookingRepository.GetByBookingID(int.Parse(bookingId));
                List<int> ChildList = ConvertHelpers.ConvertChildrenToListInt((List<Child>)booking!.Children);
                List<int> VaccinesList = booking.Vaccines.Select(x => x.Id).ToList();
                List<int> ComboList = booking.Combos.Select(x => x.Id).ToList();
                AddVaccinesTrackingRequest addVaccinesTrackingRequest = new()
                {
                    UserId = booking.ParentId,
                    VaccinationDate = booking.ArrivedAt,
                    AdministeredBy = 0
                };
                if (!VaccinesList.IsNullOrEmpty())
                    await _vaccineTrackingService.AddVaccinesToVaccinesTrackingAsync(addVaccinesTrackingRequest, VaccinesList, ChildList, booking!.Id);

                if (!ComboList.IsNullOrEmpty())
                    await _vaccineTrackingService.AddVaccinesComboToVaccinesTrackingAsync(addVaccinesTrackingRequest, ComboList, ChildList, booking!.Id);

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
                item.Amount = (int)amount * item.ChildrenList!.Count();
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
                item.amount = (int)(amount * item.ChildrenList!.Count());
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
                item.amount = (int)(amount * item.ChildrenList!.Count());
            }

            return bookingResponses;
        }
    }
}