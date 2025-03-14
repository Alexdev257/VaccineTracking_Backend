using ClassLib.DTO.VaccineTracking;
using ClassLib.Enum;
using ClassLib.Helpers;
using ClassLib.Models;
using ClassLib.Repositories;
using Microsoft.IdentityModel.Tokens;
using TimeProvider = ClassLib.Helpers.TimeProvider;

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
            return ConvertHelpers.convertToVaccinesTrackingResponse(vt!);
        }

        public async Task<int> SoftDeleteByBookingId(int id)
        {
            return await _vaccinesTrackingRepository.SoftDeleteByBookingID(id);
        }


        public async Task<bool> AddVaccinesComboToVaccinesTrackingAsync(AddVaccinesTrackingRequest request, List<int> vaccinesCombo, List<int> child, int bookingId)
        {
            if (vaccinesCombo.IsNullOrEmpty()) return false;
            foreach (var vaccinesComboID in vaccinesCombo)
            {
                var vaccineIDList = await _vaccineComboRepository.GetAllVaccineInVaccinesComboByID(vaccinesComboID);
                await AddVaccinesToVaccinesTrackingAsync(request, vaccineIDList.Select(v => v.Id).ToList(), child, bookingId);
            }
            return true;
        }
        public async Task<bool> AddVaccinesToVaccinesTrackingAsync(AddVaccinesTrackingRequest request, List<int> vaccines, List<int> child, int bookingId)
        {
            VaccinesTracking previousVaccination = null!;
            if (vaccines.IsNullOrEmpty()) return false;
            foreach (var childID in child)
            {
                foreach (var vaccinesID in vaccines)
                {
                    var vaccine = await _vaccineRepository.GetById(vaccinesID);

                    for (int dosesTimes = 1; dosesTimes <= vaccine!.DoesTimes; dosesTimes++)
                    {
                        var vt = ConvertHelpers.convertToVaccinesTrackingModel(request, childID, vaccine, previousVaccination!, bookingId);
                        await _vaccinesTrackingRepository.AddVaccinesTrackingAsync(vt);
                        previousVaccination = (await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(vt.Id))!;
                    }

                    previousVaccination = null!;
                }
            }
            return true;
        }

        public async Task<bool> UpdateVaccinesTrackingAsync(int id, UpdateVaccineTracking updateVaccineTracking)
        {
            var vt = await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(id);
            if (vt == null) return false;
            var vaccine = _vaccineRepository.GetById(vt!.VaccineId).Result;
            int checkpointForThisVaccine = 1;
            while (vt != null)
            {
                if (checkpointForThisVaccine == 1)
                {
                    vt.Status = updateVaccineTracking?.Status! ?? vt.Status;
                    vt.Reaction = string.IsNullOrEmpty(updateVaccineTracking?.Reaction) ? vt.Reaction : updateVaccineTracking.Reaction;
                    vt.AdministeredBy = (updateVaccineTracking?.AdministeredBy == 0) ? vt.AdministeredBy : updateVaccineTracking!.AdministeredBy;
                    vt.VaccinationDate = updateVaccineTracking.Reschedule ?? vt.VaccinationDate;

                    if (vt.PreviousVaccination == 0 && updateVaccineTracking.Reschedule != null)
                    {
                        vt.MinimumIntervalDate = updateVaccineTracking!.Reschedule.Value.AddDays(2);
                        vt.MaximumIntervalDate = updateVaccineTracking!.Reschedule.Value.AddDays(7);
                    }
                    else if (updateVaccineTracking.Reschedule != null)
                    {
                        vt.MinimumIntervalDate = updateVaccineTracking!.Reschedule.Value.AddDays(vaccine!.MinimumIntervalDate!.Value);
                        vt.MaximumIntervalDate = updateVaccineTracking!.Reschedule.Value.AddDays(vaccine!.MaximumIntervalDate!.Value);
                    }

                    if (updateVaccineTracking?.Status?.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Success).ToString().ToLower())
                    {
                        await _vaccineRepository.DecreseQuantityVaccines(vaccine!, 1);
                        vt.VaccinationDate = TimeProvider.GetVietnamNow();
                    }
                }
                if (updateVaccineTracking?.Status?.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Success).ToString().ToLower() && checkpointForThisVaccine == 2)
                {
                    vt.VaccinationDate = null;
                    vt.MinimumIntervalDate = TimeProvider.GetVietnamNow().AddDays(vaccine!.MinimumIntervalDate.HasValue ? vaccine.MinimumIntervalDate.Value : 30);
                    vt.MaximumIntervalDate = TimeProvider.GetVietnamNow().AddDays(vaccine!.MaximumIntervalDate.HasValue ? vaccine.MaximumIntervalDate.Value : 60);

                    vt.Status = ((VaccinesTrackingEnum)VaccinesTrackingEnum.Schedule).ToString();
                }
                if (updateVaccineTracking?.Status?.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Cancel).ToString().ToLower())
                {
                    vt.VaccinationDate = null;
                    vt.MinimumIntervalDate = null;
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

        public async Task<List<VaccinesTrackingResponse>> GetByBookingId(int id)
        {
            var vaccinesTrackings = await _vaccinesTrackingRepository.GetVaccinesTrackingByBookingID(id);
            return vaccinesTrackings.Select(vt => ConvertHelpers.convertToVaccinesTrackingResponse(vt)).ToList();
        }

        public async Task<bool> VaccinesTrackingRefund(int bookingID, VaccinesTrackingEnum vaccinesTrackingEnum = VaccinesTrackingEnum.Cancel)
        {
            var vaccinesTrackingList = await _vaccinesTrackingRepository.GetVaccinesTrackingByBookingID(bookingID);

            var updateDetails = new UpdateVaccineTracking() { Status = vaccinesTrackingEnum.ToString() };

            foreach (var item in vaccinesTrackingList)
            {
                await UpdateVaccinesTrackingAsync(item.Id, updateDetails);
            }

            return true;
        }
    }
}