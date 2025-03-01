using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        public BookingService(BookingRepository bookingRepository, UserRepository userRepository, VaccinesTrackingService vaccinesTrackingService)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _vaccineTrackingService = vaccinesTrackingService;
        }

        public async Task<List<Booking>?> GetByQuerry(BookingQuerryObject query)
        {
            return await _bookingRepository.GetByQuerry(query);
        }

        public async Task<OrderInfoModel?> AddBooking(AddBooking addBooking)
        {
            Booking booking = new()
            {
                ParentId = addBooking.ParentId,
                AdvisoryDetails = addBooking.AdvisoryDetail,
                ArrivedAt = addBooking.ArrivedAt,
                CreatedAt = DateTime.Now,
                Status = "Pending"
            };

            await _bookingRepository.AddBooking(booking, addBooking.ChildrenIds!, addBooking.vaccineIds, addBooking.vaccineComboIds);
            var user = await _userRepository.getUserByIdAsync(addBooking.ParentId);

            AddVaccinesTrackingRequest addVaccinesTrackingRequest = new()
            {
                UserId = addBooking.ParentId,
                VaccinationDate = addBooking.ArrivedAt,
                AdministeredBy = 1
            };
            if (!addBooking.vaccineIds.IsNullOrEmpty())
                await _vaccineTrackingService.AddVaccinesToVaccinesTrackingAsync(addVaccinesTrackingRequest, addBooking.vaccineIds, addBooking.ChildrenIds!);
            if (!addBooking.vaccineComboIds.IsNullOrEmpty())
                await _vaccineTrackingService.AddVaccinesComboToVaccinesTrackingAsync(addVaccinesTrackingRequest, addBooking.vaccineComboIds, addBooking.ChildrenIds!);



            return new OrderInfoModel
            {
                GuestName = user?.Name!,
                GuestEmail = user?.Gmail,
                GuestPhone = user?.PhoneNumber,
                BookingID = booking.Id.ToString(),
                OrderId = booking.Id.ToString(),
                OrderDescription = booking.AdvisoryDetails,
                Amount = addBooking.TotalPrice
            };
        }

        public async Task<Booking?> UpdateBookingStatus(string bookingId, string msg)
        {
            return await _bookingRepository.UpdateBooking(bookingId, msg);
        }
    }
}