using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories.BookingDetails;
using ClassLib.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using TimeProvider = ClassLib.Helpers.TimeProvider;

namespace ClassLib.Repositories
{
    public class BookingRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        private readonly BookingChildIdRepository _bookingChildIdRepository;
        private readonly BookingIdVaccineIdReponsitory _bookingIdVaccineIdReponsitory;
        private readonly BookingComboIdReponsitory _bookingComboIdReponsitory;
        private readonly ChildService _childService;
        public BookingRepository(DbSwpVaccineTrackingFinalContext context,
                                 BookingChildIdRepository bookingChildIdRepository,
                                 BookingIdVaccineIdReponsitory bookingIdVaccineIdReponsitory,
                                 BookingComboIdReponsitory bookingComboIdReponsitory,
                                 ChildService childService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _bookingChildIdRepository = bookingChildIdRepository ?? throw new ArgumentNullException(nameof(bookingChildIdRepository));
            _bookingIdVaccineIdReponsitory = bookingIdVaccineIdReponsitory ?? throw new ArgumentNullException(nameof(bookingIdVaccineIdReponsitory));
            _bookingComboIdReponsitory = bookingComboIdReponsitory ?? throw new ArgumentNullException(nameof(bookingComboIdReponsitory));
            _childService = childService;
        }

        // Staff
        public async Task<List<Booking>> GetAll()
        {
            return await _context.Bookings
                        .Include(x => x.Parent)
                        .Include(x => x.Children)
                        .Include(x => x.Combos)
                            .ThenInclude(x => x.Vaccines)
                        .Include(x => x.Vaccines)
                        .Include(x => x.Payments)
                            .ThenInclude(x => x.PaymentMethodNavigation)
                        .ToListAsync();
        }

        // Staff
        public async Task<Booking?> GetByBookingID(int id) => await _context.Bookings
                                                                .Include(x => x.Parent)
                                                                .Include(x => x.Combos)
                                                                    .ThenInclude(x => x.Vaccines)
                                                                .Include(x => x.Vaccines)
                                                                .Include(x => x.Children)
                                                                .Include(x => x.Payments)
                                                                .FirstOrDefaultAsync(x => x.Id == id);

        // For user
        public async Task<List<Booking>?> GetAllBookingByUserId(int userId)
        {
            var bookings = await GetAllBookingByUserIdStaff(userId);
            return bookings?.ToList();
        }

        // For staff
        public async Task<List<Booking>?> GetAllBookingByUserIdStaff(int userId)
        {
            var bookings = await GetAll();
            return bookings?.Where(b => b.ParentId == userId).ToList();
        }

        public async Task<Booking?> AddBooking(Booking booking, List<int> ChildrenIDs, List<int> VaccineIDs, List<int> VaccineComboIDs)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // This is new booking
                if (booking.Id == 0)
                {
                    _context.Bookings.Add(booking);
                    if (!ChildrenIDs.IsNullOrEmpty()) await _bookingChildIdRepository.Add(booking, ChildrenIDs);
                    if (!VaccineIDs.IsNullOrEmpty()) await _bookingIdVaccineIdReponsitory.Add(booking, VaccineIDs);
                    if (!VaccineComboIDs.IsNullOrEmpty()) await _bookingComboIdReponsitory.Add(booking, VaccineComboIDs);
                    await _childService.UpdateListChild(ChildrenIDs, "Tracking");

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return booking;
                }

                // This is existed booking but the Staff change vaccine for User
                else
                {
                    await _bookingChildIdRepository.ClearAndAdd(booking, ChildrenIDs);
                    await _bookingIdVaccineIdReponsitory.ClearAndAdd(booking, VaccineIDs);
                    await _bookingComboIdReponsitory.ClearAndAdd(booking, VaccineComboIDs);
                    await _childService.UpdateListChild(ChildrenIDs, "Tracking");
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return booking;
                }
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
            // Update booking status
            var booking = await _context.Bookings.FindAsync(int.Parse(id));
            if (booking == null)
            {
                return null;
            }
            if (msg.ToLower() == BookingEnum.Refund.ToString().ToLower())
            {
                booking.Status = BookingEnum.Refund.ToString();
            }
            else if (msg.ToLower() == BookingEnum.Success.ToString().ToLower())
            {
                booking.Status = BookingEnum.Success.ToString();
            }
            else if (msg.ToLower() == BookingEnum.Pending.ToString().ToLower())
            {
                booking.Status = BookingEnum.Pending.ToString();
            }
            else
            {
                return null;
            }

            await _context.SaveChangesAsync();
            return booking;
        }

        //Get all elements by status
        public async Task<List<Booking>> GetAllByStatus(BookingEnum bookingEnum) => await _context.Bookings.Where(x => x.Status.ToLower() == bookingEnum.ToString().ToLower()).ToListAsync();

        //Soft delete by status
        public async Task<bool> SoftDeleteStatus(BookingEnum bookingEnum)
        {
            var resultList = await GetAllByStatus(bookingEnum);

            if (resultList == null) return false;

            foreach (var item in resultList)
            {
                item.IsDeleted = true;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        //Get all element by dayrange
        public async Task<List<Booking>> GetAllByDayRange(int day) => await _context.Bookings.Where(x => x.CreatedAt < (TimeProvider.GetVietnamNow()).AddDays(-day)).ToListAsync();

        //Soft delete by day range
        public async Task<bool> SoftDeleteByDayRange(int range)
        {
            var resultList = await GetAllByDayRange(range);

            if (resultList == null) return false;

            foreach (var item in resultList)
            {
                item.IsDeleted = true;
            }
            await _context.SaveChangesAsync();
            return true;
        }


        // Hard delete
        public async Task<int> HardDelete()
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var deletedBookings = await _context.Bookings.Where(b => b.IsDeleted).ToListAsync();
                _context.Bookings.RemoveRange(deletedBookings);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return deletedBookings.Count();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                await transaction.RollbackAsync();
                return 0;
            }
        }
    }
}