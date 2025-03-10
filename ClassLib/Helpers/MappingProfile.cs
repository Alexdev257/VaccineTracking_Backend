using AutoMapper;
using ClassLib.DTO.Booking;
using ClassLib.DTO.Child;
using ClassLib.DTO.Feedback;
using ClassLib.DTO.User;
using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;
using ClassLib.Models;

namespace ClassLib.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterRequest, User>();
            CreateMap<User, RegisterResponse>()
                 .ForMember(d => d.DateOfBirth, o => o.MapFrom(src => src.DateOfBirth.ToString("HH:mm:ss dd-MM-yyyy")))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(src => src.CreatedAt.ToString("HH:mm:ss dd-MM-yyyy")));

            CreateMap<LoginRequest, User>();
            CreateMap<User, LoginResponse>();

            CreateMap<User, GetUserResponse>()
                .ForMember(d => d.DateOfBirth, o => o.MapFrom(src => src.DateOfBirth.ToString("HH:mm:ss dd-MM-yyyy")))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(src => src.CreatedAt.ToString("HH:mm:ss dd-MM-yyyy")));

            CreateMap<CreateStaffRequest, User>();

            CreateMap<CreateChildRequest, Child>();


            CreateMap<Child, GetChildResponse>()
                .ForMember(d => d.DateOfBirth,o => o.MapFrom(src =>  src.DateOfBirth.ToString("HH:mm:ss dd-MM-yyyy")))
                .ForMember(d => d.CreatedAt, o => o.MapFrom(src => src.CreatedAt.ToString("HH:mm:ss dd-MM-yyyy")));

            CreateMap<Booking, UpdateBooking>();

            CreateMap<CreateVaccine, Vaccine>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            //CreateMap<UpdateVaccine, Vaccine>();
            CreateMap<UpdateVaccine, Vaccine>()
                    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Vaccine, GetVaccine>()
                .ForMember(d => d.EntryDate, o => o.MapFrom(src => src.EntryDate.ToString("HH:mm:ss dd-MM-yyyy")))
                .ForMember(d => d.TimeExpired, o => o.MapFrom(src => src.TimeExpired.ToString("HH:mm:ss dd-MM-yyyy")));


            CreateMap<CreateVaccineCombo, VaccinesCombo>().ForMember(d => d.Vaccines, o => o.Ignore()).ReverseMap();//hai chieu.
            CreateMap<UpdateVaccineCombo, VaccinesCombo>();

            CreateMap<VaccinesCombo, GetAllVaccineCombo>();

            CreateMap<Vaccine, GetVaccineInVaccineComboDetail>();

            CreateMap<Feedback, GetFeedbackResponse>();

            CreateMap<CreateFeedbackRequest, Feedback>();
            CreateMap<UpdateFeedbackRequest, Feedback>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }

}
