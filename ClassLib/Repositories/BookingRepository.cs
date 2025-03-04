
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using ClassLib.DTO.Booking;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories.BookingDetails;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClassLib.Repositories
{
    public class BookingRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        private readonly BookingChildIdRepository _bookingChildIdRepository;
        private readonly BookingIdVaccineIdReponsitory _bookingIdVaccineIdReponsitory;
        private readonly BookingComboIdReponsitory _bookingComboIdReponsitory;
        private readonly VaccinesTrackingRepository _vaccinesTrackingRepository;
        public BookingRepository(DbSwpVaccineTrackingFinalContext context,
                                 BookingChildIdRepository bookingChildIdRepository,
                                 BookingIdVaccineIdReponsitory bookingIdVaccineIdReponsitory,
                                 BookingComboIdReponsitory bookingComboIdReponsitory,
                                 VaccinesTrackingRepository vaccinesTrackingRepository)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _bookingChildIdRepository = bookingChildIdRepository ?? throw new ArgumentNullException(nameof(bookingChildIdRepository));
            _bookingIdVaccineIdReponsitory = bookingIdVaccineIdReponsitory ?? throw new ArgumentNullException(nameof(bookingIdVaccineIdReponsitory));
            _bookingComboIdReponsitory = bookingComboIdReponsitory ?? throw new ArgumentNullException(nameof(bookingComboIdReponsitory));
            _vaccinesTrackingRepository = vaccinesTrackingRepository ?? throw new ArgumentNullException(nameof(vaccinesTrackingRepository));
        }
        public async Task<List<Booking>> GetAll()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<List<Booking>?> GetByQuerry(BookingQuerryObject bookingQuerryObject)
        {
            var booking = _context.Bookings
                          .Include(x => x.Parent)
                          .AsQueryable();

            if (bookingQuerryObject.Id.HasValue)
            {
                booking = booking.Where(x => x.Id == bookingQuerryObject.Id);
            }
            if (bookingQuerryObject.ParentId.HasValue)
            {
                booking = booking.Where(x => x.ParentId == bookingQuerryObject.ParentId);
            }
            if (bookingQuerryObject.Status != null)
            {
                booking = booking.Where(x => x.Status == bookingQuerryObject.Status);
            }
            if (bookingQuerryObject.CreateDate.HasValue)
            {
                booking = booking.Where(x => x.CreatedAt == bookingQuerryObject.CreateDate);
            }
            if (bookingQuerryObject.PhoneNumber != null)
            {
                booking = booking.Where(x => x.Parent.PhoneNumber == bookingQuerryObject.PhoneNumber);
            }
            if (bookingQuerryObject.orderBy != null)
            {
                switch (bookingQuerryObject.orderBy)
                {
                    case "Id":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.Id) : booking.OrderBy(x => x.Id);
                        break;
                    case "ParentId":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.ParentId) : booking.OrderBy(x => x.ParentId);
                        break;
                    case "CreatedAt":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.CreatedAt) : booking.OrderBy(x => x.CreatedAt);
                        break;
                    case "ArrivedAt":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.ArrivedAt) : booking.OrderBy(x => x.ArrivedAt);
                        break;
                    case "Status":
                        booking = bookingQuerryObject.isDescending ? booking.OrderByDescending(x => x.Status) : booking.OrderBy(x => x.Status);
                        break;
                }
            }

            return await booking.ToListAsync();
        }

        public async Task<Booking?> AddBooking(Booking booking, List<int> ChildrenIDs, List<int> VaccineIDs, List<int> VaccineComboIDs)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Bookings.Add(booking);
                if (!ChildrenIDs.IsNullOrEmpty()) await _bookingChildIdRepository.Add(booking, ChildrenIDs);
                if (!VaccineIDs.IsNullOrEmpty()) await _bookingIdVaccineIdReponsitory.Add(booking, VaccineIDs);
                if (!VaccineComboIDs.IsNullOrEmpty()) await _bookingComboIdReponsitory.Add(booking, VaccineComboIDs);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return booking;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<Booking?> UpdateBooking(string id, string msg)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings.FindAsync(int.Parse(id));
                if (booking == null)
                {
                    return null;
                }
                if (msg.ToLower() == "cancel")
                {
                    booking.Status = BookingEnum.Cancel.ToString();
                }
                else if (msg.ToLower() == "success")
                {
                    booking.Status = BookingEnum.Success.ToString();
                }
                else if (msg.ToLower() == "pending")
                {
                    booking.Status = BookingEnum.Pending.ToString();
                }
                else
                {
                    return null;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return booking;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                await transaction.RollbackAsync();
                return null;
            }
        }
    }
}