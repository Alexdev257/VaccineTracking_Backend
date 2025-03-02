using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Child;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Models;
using ClassLib.Repositories;
using ClassLib.Helpers;
using ClassLib.Enum;

namespace ClassLib.Service
{
    public class VaccinesTrackingService
    {
        private readonly VaccinesTrackingRepository _vaccinesTrackingRepository;

        private readonly VaccineRepository _vaccineRepository;

        private readonly VaccineComboRepository _vaccineComboRepository;
        public VaccinesTrackingService(VaccinesTrackingRepository vaccinesTrackingRepository,
                                    VaccineRepository vaccineRepository,
                                    VaccineComboRepository vaccineComboRepository)
        {
            _vaccinesTrackingRepository = vaccinesTrackingRepository;
            _vaccineRepository = vaccineRepository;
            _vaccineComboRepository = vaccineComboRepository;
        }

        public async Task<List<VaccinesTrackingResponse>> GetVaccinesTrackingAsync()
        {
            var vaccinesTrackings = await _vaccinesTrackingRepository.GetVaccinesTrackingAsync();
            return vaccinesTrackings.Select(vt => ConvertHelpers.convertToVaccinesTrackingResponse(vt)).ToList();
        }

        public async Task<List<VaccinesTrackingResponse>> GetVaccinesTrackingByParentIdAsync(int id)
        {
            var vaccinesTrackings = await _vaccinesTrackingRepository.GetVaccinesTrackingByParentIdAsync(id);
            return vaccinesTrackings.Select(vt => ConvertHelpers.convertToVaccinesTrackingResponse(vt)).ToList();
        }

        public async Task<VaccinesTrackingResponse> GetVaccinesTrackingByIdAsync(int id)
        {
            var vt = await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(id);
            return ConvertHelpers.convertToVaccinesTrackingResponse(vt);
        }

        public async Task<bool> AddVaccinesComboToVaccinesTrackingAsync(AddVaccinesTrackingRequest request, List<int> vaccinesCombo, List<int> child)
        {
            foreach (var vaccinesComboID in vaccinesCombo)
            {
                var vaccineIDList = await _vaccineComboRepository.GetAllVaccineInVaccinesComboByID(vaccinesComboID);
                await AddVaccinesToVaccinesTrackingAsync(request, vaccineIDList.Select(v => v.Id).ToList(), child);
            }
            return true;
        }
        public async Task<bool> AddVaccinesToVaccinesTrackingAsync(AddVaccinesTrackingRequest request, List<int> vaccines, List<int> child)
        {
            VaccinesTracking previousVaccination = null!;

            foreach (var childID in child)
            {
                foreach (var vaccinesID in vaccines)
                {
                    var vaccine = await _vaccineRepository.GetById(vaccinesID);

                    for (int dosesTimes = 1; dosesTimes <= vaccine!.DoesTimes; dosesTimes++)
                    {
                        var vt = ConvertHelpers.convertToVaccinesTrackingModel(request, childID, vaccine, previousVaccination!);
                        await _vaccinesTrackingRepository.AddVaccinesTrackingAsync(vt);
                        previousVaccination = await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(vt.Id);
                    }

                    previousVaccination = null!;
                }
            }
            return true;
        }


        public async Task<bool> UpdateVaccinesTrackingAsync(int id, UpdateVaccineTracking updateVaccineTracking)
        {
            var vt = await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(id);
            var vaccine = _vaccineRepository.GetById(vt!.VaccineId).Result;
            int checkpointForThisVaccine = 1;
            while (vt != null)
            {
                if (checkpointForThisVaccine == 1)
                {
                    vt.Status = updateVaccineTracking.Status ?? vt.Status;
                    vt.Reaction = updateVaccineTracking.Reaction ?? vt.Reaction;
                    vt.AdministeredBy = (updateVaccineTracking.AdministeredBy == 0) ? vt.AdministeredBy : updateVaccineTracking.AdministeredBy;
                    if (updateVaccineTracking.Status!.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Success).ToString().ToLower())
                        await _vaccineRepository.DecreseQuantityVaccines(vaccine!, 1);
                }
                if (updateVaccineTracking.Status!.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Success).ToString().ToLower())
                {
                    vt.VaccinationDate = DateTime.Now;
                    vt.MinimumIntervalDate = vt.VaccinationDate!.Value.AddDays(vaccine!.MinimumIntervalDate!.Value);
                    vt.MaximumIntervalDate = vt.VaccinationDate!.Value.AddDays(vaccine!.MaximumIntervalDate!.Value);
                }
                if (updateVaccineTracking.Status!.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Cancel).ToString().ToLower())
                {
                    vt.VaccinationDate = null;
                    vt.MinimumIntervalDate = DateTime.Now;
                    vt.MaximumIntervalDate = null;
                    vt.Status = updateVaccineTracking.Status;
                    vt.Reaction = updateVaccineTracking.Reaction ?? vt.Reaction;
                }
                checkpointForThisVaccine++;
                var vtUpdated = await _vaccinesTrackingRepository.UpdateVaccinesTrackingAsync(vt);
                vt = await _vaccinesTrackingRepository.GetVaccinesTrackingByPreviousVaccination(vtUpdated.Id);
            }

            return true;
        }
    }
}