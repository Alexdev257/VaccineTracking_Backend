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
            CreateMap<User, RegisterResponse>();

            CreateMap<LoginRequest, User>();
            CreateMap<User, LoginResponse>();

            CreateMap<User, GetUserResponse>();

            CreateMap<CreateStaffRequest, User>();

            CreateMap<CreateChildRequest, Child>();


            CreateMap<Child, GetChildResponse>();

            CreateMap<Booking, UpdateBooking>();

            CreateMap<CreateVaccine, Vaccine>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            //CreateMap<UpdateVaccine, Vaccine>();
            CreateMap<UpdateVaccine, Vaccine>()
                    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Vaccine, GetVaccine>();


            CreateMap<CreateVaccineCombo, VaccinesCombo>().ForMember(d => d.Vaccines, o => o.Ignore()).ReverseMap();//hai chieu.
            CreateMap<UpdateVaccineCombo, VaccinesCombo>();

            CreateMap<VaccinesCombo, GetAllVaccineCombo>();

            CreateMap<Vaccine, GetVaccineInVaccineComboDetail>();

            CreateMap<Feedback, GetFeedbackResponse>();
            CreateMap<Feedback, GetFeedbackAdminResponse>();

            CreateMap<CreateFeedbackRequest, Feedback>();
            CreateMap<UpdateFeedbackRequest, Feedback>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }

}
