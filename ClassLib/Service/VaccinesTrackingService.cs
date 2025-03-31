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

        private readonly ChildService _childServices;
        public VaccinesTrackingService(VaccinesTrackingRepository vaccinesTrackingRepository,
                                    VaccineRepository vaccineRepository,
                                    VaccineComboRepository vaccineComboRepository,
                                    ChildService childService)
        {
            _vaccinesTrackingRepository = vaccinesTrackingRepository;
            _vaccineRepository = vaccineRepository;
            _vaccineComboRepository = vaccineComboRepository;
            _childServices = childService;
        }

        public async Task<List<VaccinesTrackingResponse>> GetVaccinesTrackingAsync()
        {
            var vaccinesTrackings = await _vaccinesTrackingRepository.GetVaccinesTrackingAsync();
            return vaccinesTrackings.Select(vt => ConvertHelpers.convertToVaccinesTrackingResponse(vt)).ToList();
        }
        public async Task<List<VaccinesTrackingResponse>> GetVaccinesTrackingAsyncStaff()
        {
            var vaccinesTrackings = await _vaccinesTrackingRepository.GetVaccinesTrackingStaffAsync();
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

        public async Task<int> HardDeleteByBookingId(int id)
        {
            return await _vaccinesTrackingRepository.HardDeleteByBookingId(id);
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
                        await _vaccineRepository.DecreseQuantityVaccines(vaccine!, 1);
                    }

                    previousVaccination = null!;
                }
            }
            return true;
        }

        public async Task<bool> UpdateVaccinesTrackingAsync(int id, UpdateVaccineTracking updateVaccineTracking)
        {
            var vt = await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(id);


            var holdChildID = vt!.ChildId;
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
                        vt.VaccinationDate = TimeProvider.GetVietnamNow();
                    }
                }
                if (updateVaccineTracking?.Status?.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Success).ToString().ToLower() && checkpointForThisVaccine == 2)
                {
                    vt.MinimumIntervalDate = TimeProvider.GetVietnamNow().AddDays(vaccine!.MinimumIntervalDate ?? 30);
                    vt.MaximumIntervalDate = TimeProvider.GetVietnamNow().AddDays(vaccine!.MaximumIntervalDate ?? 60);
                    DateTime minDate = vt.MinimumIntervalDate ?? TimeProvider.GetVietnamNow();
                    DateTime maxDate = vt.MaximumIntervalDate ?? TimeProvider.GetVietnamNow();
                    vt.VaccinationDate = minDate.AddDays((maxDate - minDate).TotalDays / 2);
                    vt.Status = ((VaccinesTrackingEnum)VaccinesTrackingEnum.Schedule).ToString();
                }
                if (updateVaccineTracking?.Status?.ToLower() == ((VaccinesTrackingEnum)VaccinesTrackingEnum.Cancel).ToString().ToLower())
                {
                    vt.VaccinationDate = null;
                    vt.MinimumIntervalDate = null;
                    vt.MaximumIntervalDate = null;
                    vt.Status = updateVaccineTracking.Status;
                    vt.Reaction = updateVaccineTracking.Reaction ?? vt.Reaction;
                    await _vaccineRepository.IncreseQuantityVaccines(vaccine!, 1);
                }
                checkpointForThisVaccine++;
                var vtUpdated = await _vaccinesTrackingRepository.UpdateVaccinesTrackingAsync(vt);
                vt = await _vaccinesTrackingRepository.GetVaccinesTrackingByPreviousVaccination(vtUpdated.Id);
            }
            var isTracking = await _vaccinesTrackingRepository.CheckIsChildrenTracking(holdChildID);

            if (!isTracking)
            {
                await _childServices.UpdateOnlyChild(holdChildID, "Active");
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

        //Alex5
        public async Task<List<VaccinesTracking>> GetUpcomingVaccinationsReminderAsync()
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            var list = await _vaccinesTrackingRepository.GetUpComingVaccinations(today);
            if (list.Count == 0)
            {
                throw new ArgumentException("No more reminder for upcoming");
            }
            return list;
        }

        public async Task<List<VaccinesTracking>> GetDeadlineVaccinationsReminderAsync()
        {
            var today = Helpers.TimeProvider.GetVietnamNow();
            var list = await _vaccinesTrackingRepository.GetDeadlineVaccinations(today);
            if (list.Count == 0)
            {
                throw new ArgumentException("No more reminder for upcoming");
            }
            return list;
        }
    }
}