using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ClassLib.Models;
using ClassLib.DTO.Booking;

namespace SWP391_BackEnd
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Booking, UpdateBooking>();
        }
    }
}