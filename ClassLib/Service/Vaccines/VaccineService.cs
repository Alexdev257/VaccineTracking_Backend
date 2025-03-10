using AutoMapper;
using ClassLib.DTO.Vaccine;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service.Vaccines
{
    public class VaccineService
    {
        private readonly VaccineRepository _vaccineRepository;
        private readonly IMapper _mapper;

        public VaccineService(VaccineRepository vaccineRepository, IMapper mapper)
        {
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        //Lấy tất cả
        public async Task<List<GetVaccine>> GetAllVaccines()
        {
            var listVaccine = await _vaccineRepository.GetAllVaccines();
            return _mapper.Map<List<GetVaccine>>(listVaccine);
        }

        //Alex5
        //public async Task<List<GetVaccine>> GetAllVaccinesAdmin()
        //{
        //    var listVaccine = await _vaccineRepository.GetAllVaccinesAdmin();
        //    return _mapper.Map<List<GetVaccine>>(listVaccine);
        //}

        //Lấy theo id
        public async Task<GetVaccine?> GetVaccineById(int id)
        {
            var vaccine = await _vaccineRepository.GetById(id);
            return _mapper.Map<GetVaccine>(vaccine);
        }

        //Alex5
        //public async Task<GetVaccine?> GetVaccineByIdAdmin(int id)
        //{
        //    var vaccine = await _vaccineRepository.GetByIdAdmin(id);
        //    return _mapper.Map<GetVaccine>(vaccine);
        //}

        //Tạo mới
        public async Task<Vaccine> CreateVaccine(CreateVaccine rq)
        {
            var rs = _mapper.Map<Vaccine>(rq);
            rs.IsDeleted = false;
            return await _vaccineRepository.CreateVaccine(rs);
        }

        //Update
        //public async Task<Models.Vaccine> UpdateVaccine(UpdateVaccine rq, int id)
        //{
        //    var currentVaccine = await _vaccineRepository.GetById(id);
        //    if (currentVaccine == null)
        //    {
        //        throw new ArgumentException(nameof(currentVaccine));
        //    }
        //    return await _vaccineRepository.UpdateVaccine(currentVaccine, _mapper.Map<Models.Vaccine>(rq));
        //}

        //Alex5
        public async Task<bool> UdateVaccineAsync(int id, UpdateVaccine request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentNullException("Name can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Quantity.ToString()))
            {
                throw new ArgumentNullException("Quantity can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentNullException("Description can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Price.ToString()))
            {
                throw new ArgumentNullException("Price can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.DoesTimes.ToString()))
            {
                throw new ArgumentNullException("DosesTimes can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.SuggestAgeMin.ToString()))
            {
                throw new ArgumentNullException("Suggest min age can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.SuggestAgeMax.ToString()))
            {
                throw new ArgumentNullException("Suggest max age can not be blank");
            }
            //public DateTime EntryDate { get; set; }
            if (string.IsNullOrWhiteSpace(request.TimeExpired.ToString()))
            {
                throw new ArgumentNullException("Time expired can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.AddressId.ToString()))
            {
                throw new ArgumentNullException("Address can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.Status))
            {
                throw new ArgumentNullException("Status can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.MinimumIntervalDate.ToString()))
            {
                throw new ArgumentNullException("MinimumIntervalDate can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.MaximumIntervalDate.ToString()))
            {
                throw new ArgumentNullException("MaximumIntervalDate can not be blank");
            }
            if (string.IsNullOrWhiteSpace(request.FromCountry))
            {
                throw new ArgumentNullException("FromCountry can not be blank");
            }
            var vaccine = await _vaccineRepository.GetById(id);
            if (vaccine == null)
            {
                throw new ArgumentException("Do not exist this vaccine");
            }
            else
            {
                //vaccine.Name = request.Name;
                //vaccine.Quantity = request.Quantity;
                //vaccine.Description = request.Description;
                //vaccine.Price = request.Price;
                //vaccine.DoesTimes = request.DoesTimes;
                //vaccine.SuggestAgeMin = request.SuggestAgeMin;
                //vaccine.SuggestAgeMax = request.SuggestAgeMax;
                //vaccine.TimeExpired = request.TimeExpired;
                //vaccine.AddressId = request.AddressId;
                //vaccine.Status = request.Status;
                //vaccine.MinimumIntervalDate = request.MinimumIntervalDate;
                //vaccine.MaximumIntervalDate = request.MaximumIntervalDate;
                //vaccine.FromCountry = request.FromCountry;
                var rs = _mapper.Map(request, vaccine); // Chỉ cập nhật các field có trong UpdateVaccine
                return await _vaccineRepository.UpdateVaccine(rs);
            } 
        }

        //Xoá
        public async Task<bool> DeleteVaccine(int id)
        {
            var currentVaccine = await _vaccineRepository.GetById(id);
            if (currentVaccine == null)
            {
                throw new ArgumentException(nameof(currentVaccine));
            }
            return await _vaccineRepository.DeleteVaccine(currentVaccine);
        }

        //Alex5
        public async Task<bool> SoftDeleteVaccine(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }

            var vaccine = await _vaccineRepository.GetById(id);
            if(vaccine == null)
            {
                throw new ArgumentException("Do not exist child");
            }
            vaccine.IsDeleted = true;
            return await _vaccineRepository.UpdateVaccine(vaccine);
        }
        //lay theo tuoi
        public async Task<List<Models.Vaccine>> GetVaccinesByAge(int age)
        {
            return await _vaccineRepository.GetVaccinesByAge(age);
        }
        
        //public async Task<List<Models.Vaccine>> GetVaccinesByAgeAdmin(int age)
        //{
        //    return await _vaccineRepository.GetVaccinesByAgeAdmin(age);
        //}


    }
}
