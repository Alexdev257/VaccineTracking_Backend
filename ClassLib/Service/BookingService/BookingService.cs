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
        private readonly VaccineComboRepository _vaccineComboRepository;
        private readonly VaccineRepository _vaccineRepository;
        public BookingService(BookingRepository bookingRepository
                            , UserRepository userRepository
                            , VaccinesTrackingService vaccinesTrackingService
                            , VaccineComboRepository vaccineComboRepository
                            , VaccineRepository vaccineRepository)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _vaccineTrackingService = vaccinesTrackingService;
            _vaccineComboRepository = vaccineComboRepository;
            _vaccineRepository = vaccineRepository;
        }

        public async Task<List<Booking>?> GetByQuerry(BookingQuerryObject query)
        {
            return await _bookingRepository.GetByQuerry(query);
        }

        public async Task<OrderInfoModel?> AddBooking(AddBooking addBooking)
        {
            Booking booking = ConvertHelpers.convertToBooking(addBooking);
            booking = (await _bookingRepository.AddBooking(booking, addBooking.ChildrenIds!, addBooking.vaccineIds!, addBooking.vaccineComboIds!))!;
            AddVaccinesTrackingRequest addVaccinesTrackingRequest = ConvertHelpers.convertToVaccinesTrackingRequest(addBooking);
            if (!addBooking.vaccineIds.IsNullOrEmpty())
                await _vaccineTrackingService.AddVaccinesToVaccinesTrackingAsync(addVaccinesTrackingRequest, addBooking.vaccineIds!, addBooking.ChildrenIds!, booking!.Id);
            if (!addBooking.vaccineComboIds.IsNullOrEmpty())
                await _vaccineTrackingService.AddVaccinesComboToVaccinesTrackingAsync(addVaccinesTrackingRequest, addBooking.vaccineComboIds!, addBooking.ChildrenIds!, booking!.Id);

            var user = await _userRepository.getUserByIdAsync(addBooking.ParentId);

            return ConvertHelpers.convertToOrderInfoModel(booking!, user!, addBooking);
        }

        public async Task<OrderInfoModel?> RepurchaseBooking(int bookingID)
        {
            Booking booking = (await _bookingRepository.GetByBookingID(bookingID))!;
            var amount = (await _vaccineRepository.SumMoneyOfVaccinesList((List<Vaccine>)booking.Vaccines)) + (await _vaccineComboRepository.SumMoneyOfComboList((List<VaccinesCombo>)booking.Combos));
            User user = (await _userRepository.getUserByIdAsync(booking.ParentId))!;

            return ConvertHelpers.RepurchaseBookingtoOrderInfoModel(booking, user!, (int)amount);
        }

        public async Task<Booking?> UpdateBookingStatus(string bookingId, string msg)
        {
            return await _bookingRepository.UpdateBooking(bookingId, msg);
        }

        public async Task<List<Booking>?> GetBookingByUserAsync(int id) => await _bookingRepository.GetAllBookingByUserId(id);
    }
}