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
        public async Task<List<Vaccine>> GetAllVaccines()
        {
            return await _vaccineRepository.GetAllVaccines();
        }

        //Lấy theo id
        public async Task<Vaccine?> GetVaccineById(int id)
        {
            return await _vaccineRepository.GetById(id);
        }

        //Tạo mới
        public async Task<Vaccine> CreateVaccine(CreateVaccine rq)
        {
            return await _vaccineRepository.CreateVaccine(_mapper.Map<Vaccine>(rq));
        }

        //Update
        public async Task<Vaccine> UpdateVaccine(UpdateVaccine rq, int id)
        {
            var currentVaccine = await _vaccineRepository.GetById(id);
            if (currentVaccine == null)
            {
                throw new ArgumentException(nameof(currentVaccine));
            }
            return await _vaccineRepository.UpdateVaccine(currentVaccine, _mapper.Map<Vaccine>(rq));
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
    }
}
