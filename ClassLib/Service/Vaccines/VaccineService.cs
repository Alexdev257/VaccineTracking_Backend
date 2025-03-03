using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //Lấy theo id
        public async Task<GetVaccine?> GetVaccineById(int id)
        {
            var vaccine = await _vaccineRepository.GetById(id);
            return _mapper.Map<GetVaccine>(vaccine);
        }

        //Tạo mới
        public async Task<Models.Vaccines> CreateVaccine(CreateVaccine rq)
        {
            return await _vaccineRepository.CreateVaccine(_mapper.Map<Models.Vaccines>(rq));
        }

        //Update
        public async Task<Models.Vaccines> UpdateVaccine(UpdateVaccine rq, int id)
        {
            var currentVaccine = await _vaccineRepository.GetById(id);
            if (currentVaccine == null)
            {
                throw new ArgumentException(nameof(currentVaccine));
            }
            return await _vaccineRepository.UpdateVaccine(currentVaccine, _mapper.Map<Models.Vaccines>(rq));
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
        //lay theo tuoi
        public async Task<List<Models.Vaccines>> GetVaccinesByAge(int age)
        {
            return await _vaccineRepository.GetVaccinesByAge(age);
        }


    }
}
