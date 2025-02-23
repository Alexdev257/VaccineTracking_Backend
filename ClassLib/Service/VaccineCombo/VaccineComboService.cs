using AutoMapper;
using ClassLib.DTO.Vaccine;
using ClassLib.DTO.VaccineCombo;
using ClassLib.Models;
using ClassLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib.Service.VaccineCombo
{
    public class VaccineComboService
    {
        private readonly VaccineComboRepository _vaccineComboRepository;
        private readonly IMapper _mapper;

        public VaccineComboService(VaccineComboRepository vaccineComboRepository, IMapper mapper)
        {
            _vaccineComboRepository = vaccineComboRepository ?? throw new ArgumentNullException(nameof(vaccineComboRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        //Lấy tất cả
        public async Task<List<VaccinesCombo>> GetAllVaccineCombo()
        {
            return await _vaccineComboRepository.GetAllVaccineCombo();
        }
        //Lấy theo id
        public async Task<VaccinesCombo?> GetVaccineComboById(int id)
        {
            return await _vaccineComboRepository.GetById(id);
        }
        //Tạo mới
        public async Task<VaccinesCombo> CreateVaccineCombo(CreateVaccineCombo rq)
        {
            return await _vaccineComboRepository.CreateVaccine(_mapper.Map<VaccinesCombo>(rq));
        }
        //Update
        public async Task<VaccinesCombo> UpdateVaccineCombo(UpdateVaccineCombo rq, int id)
        {
            var currentVaccineCombo = await _vaccineComboRepository.GetById(id);
            if (currentVaccineCombo == null)
            {
                throw new ArgumentException(nameof(currentVaccineCombo));
            }
            return await _vaccineComboRepository.UpdateVaccine(currentVaccineCombo, _mapper.Map<VaccinesCombo>(rq));
        }
        //xóa
        public async Task<bool> DeleteVaccineCombo(int id)
        {
            var currentVaccine = await _vaccineComboRepository.GetById(id);
            if (currentVaccine == null)
            {
                throw new ArgumentException(nameof(currentVaccine));
            }
            return await _vaccineComboRepository.DeleteVaccineCombo(currentVaccine);
        }
    }
}
