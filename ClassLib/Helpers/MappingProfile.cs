using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ClassLib.DTO.Booking;
using ClassLib.DTO.User;
using ClassLib.Models;

namespace ClassLib.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, User>();
            CreateMap<User, RegisterResponse>();

            CreateMap<LoginRequest, User>();
            CreateMap<User, LoginResponse>();

            CreateMap<User, GetUserResponse>();

            CreateMap<Booking, UpdateBooking>();

        }
    }
    
}
