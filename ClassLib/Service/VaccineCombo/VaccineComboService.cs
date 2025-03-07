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
//using ClassLib.DTO.VaccineCombo;

namespace ClassLib.Service.VaccineCombo
{
    public class VaccineComboService
    {
        private readonly VaccineComboRepository _vaccineComboRepository;
        private readonly VaccineRepository _vaccineRepository;
        private readonly IMapper _mapper;

        public VaccineComboService(VaccineComboRepository vaccineComboRepository, IMapper mapper, VaccineRepository vaccineRepository)
        {
            _vaccineComboRepository = vaccineComboRepository ?? throw new ArgumentNullException(nameof(vaccineComboRepository));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        //Lấy tất cả

        public async Task<List<GetAllVaccineCombo>> GetAllVaccineCombo()
        {
            //var listCombo = await _vaccineComboRepository.GetAllVaccineCombo();
            //List<DTO.VaccineCombo.GetAllVaccineCombo> responses = new();
            //foreach (var vaccineCombo in listCombo)
            //{
            //    DTO.VaccineCombo.GetAllVaccineCombo response = new()
            //    {
            //        Id = vaccineCombo.Id,
            //        ComboName = vaccineCombo.ComboName,
            //        Discount = vaccineCombo.Discount,
            //        FinalPrice = vaccineCombo.FinalPrice,
            //        Status = vaccineCombo.Status,
            //        TotalPrice = vaccineCombo.TotalPrice,
            //    };
            //    var listVaccine = vaccineCombo.Vaccines.ToList();
            //    response.vaccineIds = new List<int>();
            //    foreach (var item in listVaccine)
            //    {
            //        response.vaccineIds.Add(item.Id);
            //    }
            //    responses.Add(response);
            //}
            //return responses;

            var combo = await _vaccineComboRepository.GetAllVaccineCombo();
            List<GetAllVaccineCombo> responses = new List<GetAllVaccineCombo>();
            foreach (var vaccineCombo in combo)
            {
                GetAllVaccineCombo response = new GetAllVaccineCombo()
                {
                    Id = vaccineCombo.Id,
                    ComboName = vaccineCombo.ComboName,
                    Discount = vaccineCombo.Discount,
                    FinalPrice = vaccineCombo.FinalPrice,
                    Status = vaccineCombo.Status,
                    TotalPrice = vaccineCombo.TotalPrice,
                };
                var listVaccine = vaccineCombo.Vaccines.ToList();
                response.Vaccines = new List<GetVaccineInVaccineCombo>();
                foreach (var item in listVaccine)
                {
                    GetVaccineInVaccineCombo vcs = new()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = item.Price,

                    };
                    response.Vaccines.Add(vcs);
                }
                responses.Add(response);
            }
            return responses;
        }
        //Lấy theo id
        public async Task<VaccinesCombo?> GetVaccineComboById(int id)
        {
            return await _vaccineComboRepository.GetById(id);
        }

        public async Task<GetVaccineComboDetail> GetDetailVaccineComboByIdAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be balnk");
            }
            var combo = await _vaccineComboRepository.GetDetailVaccineComboById(id);
            if (combo == null)
            {
                throw new Exception("Do not exist this vaccine combo");
            }
            GetVaccineComboDetail res = new GetVaccineComboDetail
            {
                Id = combo.Id,
                ComboName = combo.ComboName,
                Discount = combo.Discount,
                TotalPrice = combo.TotalPrice,
                FinalPrice = combo.FinalPrice,
                Status = combo.Status,
                Vaccines = new List<GetVaccineInVaccineComboDetail>(),
            };
            var listVaccine = combo.Vaccines.ToList();
            foreach (var item in listVaccine)
            {
                var vcs = _mapper.Map<GetVaccineInVaccineComboDetail>(item);
                res.Vaccines.Add(vcs);
            }
            return res;
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
            var u = _mapper.Map<VaccinesCombo>(rq);
            u.Id = currentVaccineCombo.Id;
            return await _vaccineComboRepository.UpdateVaccine(currentVaccineCombo, u);
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

        public async Task<AddVaccineResponse> AddVaccine(AddVaccineIntoCombo rq, int id)
        {
            var currentCombo = await _vaccineComboRepository.GetById(id);
            if (currentCombo == null)
            {
                throw new ArgumentException(nameof(currentCombo));
            }
            var listVaccineID = rq.VaccineIds.ToList();
            var listCurrentVaccine = currentCombo.Vaccines.ToList();
            foreach (var item in listVaccineID)
            {
                bool isDuplicate = false;
                if (listCurrentVaccine.Count > 0)
                {
                    foreach (var vaccine in listCurrentVaccine)
                    {
                        if (item == vaccine.Id)
                        {
                            isDuplicate = true;
                            break;
                        }
                    }
                }
                if (!isDuplicate)
                {
                    var newVaccine = await _vaccineRepository.GetById(item);
                    currentCombo.Vaccines.Add(newVaccine);
                }
            }
            var rs = await _vaccineComboRepository.UpdateVaccineWithID(id, currentCombo);
            AddVaccineResponse response = new AddVaccineResponse()
            {
                ComboId = rs.Id,
                ComboName = rs.ComboName,
                Vaccines = rs.Vaccines.ToList(),
            };
            return response;
        }

    }
}
