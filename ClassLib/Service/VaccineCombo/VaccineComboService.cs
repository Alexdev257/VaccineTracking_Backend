﻿using AutoMapper;
using ClassLib.DTO.VaccineCombo;
using ClassLib.Models;
using ClassLib.Repositories;
using static Azure.Core.HttpHeader;
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
            var combo = await _vaccineComboRepository.GetAllVaccineCombo();
            return GetAllVaccineComboHelper(combo);

        }

        
        public async Task<List<GetAllVaccineComboByAdmin>> GetAllVaccineComboAdmin()
        {
            var combo = await _vaccineComboRepository.GetAllVaccineComboAdmin();
            return GetAllVaccineComboByAdminHelper(combo);



        }
        private List<GetAllVaccineCombo> GetAllVaccineComboHelper(List<VaccinesCombo> combos)
        {
            List<GetAllVaccineCombo> responses = new();
            foreach (var vaccineCombo in combos)
            {
                GetAllVaccineCombo response = new GetAllVaccineCombo()
                {
                    Id = vaccineCombo.Id,
                    ComboName = vaccineCombo.ComboName,
                    Discount = vaccineCombo.Discount,
                    FinalPrice = vaccineCombo.FinalPrice,
                    Status = vaccineCombo.Status,
                    TotalPrice = vaccineCombo.TotalPrice,
                    Vaccines = new List<GetVaccineInVaccineCombo>()
                };

                var listVaccine = vaccineCombo.Vaccines.ToList();
                foreach (var item in listVaccine)
                {
                    GetVaccineInVaccineCombo vcs = new()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = item.Price,
                        SuggestAgeMin = item.SuggestAgeMin,
                        SuggestAgeMax = item.SuggestAgeMax,
                    };
                    response.Vaccines.Add(vcs);
                }
                responses.Add(response);
            }
            return responses;
        }

        // Helper method cho admin view
        private List<GetAllVaccineComboByAdmin> GetAllVaccineComboByAdminHelper(List<VaccinesCombo> combos)
        {
            List<GetAllVaccineComboByAdmin> responses = new();
            foreach (var vaccineCombo in combos)
            {
                GetAllVaccineComboByAdmin response = new GetAllVaccineComboByAdmin()
                {
                    Id = vaccineCombo.Id,
                    ComboName = vaccineCombo.ComboName,
                    Discount = vaccineCombo.Discount,
                    FinalPrice = vaccineCombo.FinalPrice,
                    Status = vaccineCombo.Status,
                    TotalPrice = vaccineCombo.TotalPrice,
                    IsDeleted = vaccineCombo.IsDeleted,
                    Vaccines = new List<GetVaccineInVaccineCombo>()
                };

                var listVaccine = vaccineCombo.Vaccines.ToList();
                foreach (var item in listVaccine)
                {
                    GetVaccineInVaccineCombo vcs = new()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = item.Price,
                        SuggestAgeMin = item.SuggestAgeMin,
                        SuggestAgeMax = item.SuggestAgeMax,
                    };
                    response.Vaccines.Add(vcs);
                }
                responses.Add(response);
            }
            return responses;
        }

        //private List<GetAllVaccineCombo> GetAllVaccineComboHelper(List<VaccinesCombo> combo)
        //{
        //    List<GetAllVaccineCombo> responses = new();
        //    foreach (var vaccineCombo in combo)
        //    {
        //        GetAllVaccineCombo response = new GetAllVaccineCombo()
        //        {
        //            Id = vaccineCombo.Id,
        //            ComboName = vaccineCombo.ComboName,
        //            Discount = vaccineCombo.Discount,
        //            FinalPrice = vaccineCombo.FinalPrice,
        //            Status = vaccineCombo.Status,
        //            TotalPrice = vaccineCombo.TotalPrice,
        //        };
        //        var listVaccine = vaccineCombo.Vaccines.ToList();
        //        response.Vaccines = new List<GetVaccineInVaccineCombo>();
        //        foreach (var item in listVaccine)
        //        {
        //            GetVaccineInVaccineCombo vcs = new()
        //            {
        //                Id = item.Id,
        //                Name = item.Name,
        //                Price = item.Price,
        //                SuggestAgeMin = item.SuggestAgeMin,
        //                SuggestAgeMax = item.SuggestAgeMax,
        //            };
        //            response.Vaccines.Add(vcs);
        //        }
        //        responses.Add(response);
        //    }
        //    return responses;
        //}

        //Lấy theo id
        public async Task<VaccinesCombo?> GetVaccineComboById(int id)
        {
            return await _vaccineComboRepository.GetById(id);
        }

        //Alex5
        public async Task<GetVaccineComboDetail> GetDetailVaccineComboByIdAsync(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be balnk");
            }
            var combo = await _vaccineComboRepository.GetById(id);
            if (combo == null)
            {
                throw new Exception("Do not exist this vaccine combo");
            }
            return GetVaccineComboDetail(combo);
        }

        public async Task<GetVaccineComboDetail> GetDetailVaccineComboByIdAsyncAdmin(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be balnk");
            }
            var combo = await _vaccineComboRepository.GetByIdAdmin(id);
            if (combo == null)
            {
                throw new Exception("Do not exist this vaccine combo");
            }
            return GetVaccineComboDetail(combo);
        }

        private GetVaccineComboDetail GetVaccineComboDetail(VaccinesCombo combo)
        {

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
            var listId = rq.vaccines;
            VaccinesCombo newCombo = new();
            List<Vaccine> vaccines = new();
            foreach (var item in listId)
            {
                var vaccine = await _vaccineRepository.GetById(item);
                vaccines.Add(vaccine);
            }
            newCombo.Vaccines = vaccines;
            return await _vaccineComboRepository.CreateVaccine(_mapper.Map(rq, newCombo));
        }
        //Update
        //public async Task<VaccinesCombo> UpdateVaccineCombo(UpdateVaccineCombo rq, int id)
        //{
        //    var currentVaccineCombo = await _vaccineComboRepository.GetById(id);
        //    if (currentVaccineCombo == null)
        //    {
        //        throw new ArgumentException(nameof(currentVaccineCombo));
        //    }
        //    var u = _mapper.Map<VaccinesCombo>(rq);
        //    u.Id = currentVaccineCombo.Id;
        //    return await _vaccineComboRepository.UpdateVaccine(currentVaccineCombo, u);
        //}

        //Alex5
        public async Task<bool> UpdateVaccineCombo(int id, UpdateVaccineCombo request)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("ID can not be blank");
            }
            var combo = await _vaccineComboRepository.GetById(id);
            if (combo == null)
            {
                throw new ArgumentException("Combo does not exist");
            }
            combo.ComboName = request.ComboName;
            combo.Discount = request.Discount;
            combo.TotalPrice = request.TotalPrice;
            combo.FinalPrice = request.FinalPrice;
            //if(combo.Status.ToLower() == "Outstock".ToLower() || combo.Status.ToLower() == "Nearlyoutstock".ToLower())
            //{
            //var chekVaccine = combo.Vaccines.ToList();
            if (request.Status.ToLower() == "Instock".ToLower())
            {
                var check = combo.Vaccines.All(v => v.Status.ToLower() == "Instock".ToLower());
                if (check)
                {
                    combo.Status = request.Status;
                }
                //else
                //{
                //    combo.Status = combo.Status;
                //}
            }
            else if (request.Status.ToLower() == "Outstock".ToLower())
            {
                combo.Status = request.Status;
            }
            else if(request.Status.ToLower() == "Nearlyoutstock".ToLower())
            {
                combo.Status = request.Status;
            }
            //}

            List<int> vaccineIds = request.vaccineIds;
            //List<int> existVaccines = combo.Vaccines.Id.ToList();
            List<Vaccine> vaccines = combo.Vaccines.ToList();
            //List<int> existVaccineIds = new List<int>();
            List<int> existVaccineIds = vaccines.Select(x => x.Id).ToList();
            //foreach (var item in vaccines)
            //{
            //    existVaccineIds.Add(item.Id);
            //}
            //foreach(var item in vaccineIds)
            //{
            //    var vaccine = await _vaccineRepository.GetById(item);
            //    vaccines.Add(vaccine);
            //}
            //combo.Vaccines = vaccines;
            //if(request.Status.ToLower() == "Unavailable")
            //{
            //    combo.IsDeleted = true;
            //}else if(request.Status.ToLower() == "Available")
            //{
            //    combo.IsDeleted = false;
            //}

            // Xác định vaccine cần thêm
            var vaccinesToAdd = vaccineIds.Except(existVaccineIds).ToList();
            foreach (var vaccineId in vaccinesToAdd)
            {
                var vaccine = await _vaccineRepository.GetById(vaccineId);
                if (vaccine != null)
                {
                    combo.Vaccines.Add(vaccine);
                }
            }

            // Xác định vaccine cần xóa
            var vaccinesToRemove = vaccines.Where(v => !vaccineIds.Contains(v.Id)).ToList();
            foreach (var vaccine in vaccinesToRemove)
            {
                combo.Vaccines.Remove(vaccine);
            }
            return await _vaccineComboRepository.UpdateCombo(combo);
        }
        //xóa
        public async Task<bool> DeleteVaccineCombo(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not be blank");
            }
            var currentVaccine = await _vaccineComboRepository.GetById(id);
            if (currentVaccine == null)
            {
                throw new ArgumentException(nameof(currentVaccine));
            }
            return await _vaccineComboRepository.DeleteVaccineCombo(currentVaccine);
        }

        //Alex5
        public async Task<bool> SoftDeleteVaccineCombo(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not be blank");
            }
            var combo = await _vaccineComboRepository.GetByIdAdmin(id);
            if(combo == null)
            {
                throw new ArgumentException("Combo does not exist");
            }
            if(combo.IsDeleted == true)
            {
                throw new ArgumentException("Combo has been deleted");
            }
            combo.IsDeleted = true;
            //combo.Status = "Unvailable";
            return await _vaccineComboRepository.UpdateCombo(combo);
        }

        //Alex5
        public async Task<bool> RestoreVaccineCombo(int id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ArgumentNullException("Id can not be blank");
            }
            var combo = await _vaccineComboRepository.GetByIdAdmin(id);
            if (combo == null)
            {
                throw new ArgumentException("Combo does not exist");
            }
            if (combo.IsDeleted == false)
            {
                throw new ArgumentException("Combo has not been deleted");
            }
            combo.IsDeleted = false;
            //combo.Status = "Unvailable";
            return await _vaccineComboRepository.UpdateCombo(combo);
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

        public async Task<AddVaccineResponse> RemoveVaccine(AddVaccineIntoCombo rq, int id)
        {
            
            var changedCombo = await _vaccineComboRepository.RemoveVaccineFromCombo(id, rq.VaccineIds);
            if(changedCombo == null)
            {
                throw new ArgumentNullException();
            }
            AddVaccineResponse response = new()
            {
                ComboId = changedCombo.Id,
                ComboName = changedCombo.ComboName,
                Vaccines = changedCombo.Vaccines.ToList(),
            };
            return response;
        }

    }
}
