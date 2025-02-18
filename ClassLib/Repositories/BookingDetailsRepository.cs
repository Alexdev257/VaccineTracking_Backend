using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.BookingDetail;
using ClassLib.Models;

namespace ClassLib.Repositories
{
    public class BookingDetailsRepository
    {
        private readonly DbSwpVaccineTrackingContext _context;
        public BookingDetailsRepository(DbSwpVaccineTrackingContext context){
            _context = context ?? throw new ArgumentException(nameof(context));
        }
        // public async Task<List<BookingDetail>> AddBookingDetails(AddBookingDetails addBookingDetails){
        //     if(addBookingDetails.ChildrenIds == null){
                
        //     }
        //     else foreach(var children in addBookingDetails.ChildrenIds){

        //     }
        // }
    }
}