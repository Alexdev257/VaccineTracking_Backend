using ClassLib.DTO.Booking;
using ClassLib.DTO.Payment;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
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
        public BookingService(BookingRepository bookingRepository
                            , UserRepository userRepository
                            , VaccinesTrackingService vaccinesTrackingService
                            , PaymentMethodRepository paymentMethodRepository
                            , PaymentRepository paymentRepository)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _vaccineTrackingService = vaccinesTrackingService;
            _paymentMethodRepository = paymentMethodRepository;
            _paymentRepository = paymentRepository;
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
            return await _bookingRepository.UpdateBooking(bookingId, msg);
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

        public async Task<List<BookingResponse>?> GetBookingByUserAsyncStaff(int id)
        {
            List<BookingResponse> bookingResponses = ConvertHelpers.ConvertBookingResponse((await _bookingRepository.GetAllBookingByUserIdStaff(id))!);
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

        public async Task<List<BookingResponse>?> GetAllBookingForStaff()
        {
            List<BookingResponse> bookingResponses = ConvertHelpers.ConvertBookingResponse(await _bookingRepository.GetAll());
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
    }
}