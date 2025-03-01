using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassLib.DTO.Child;
using ClassLib.DTO.VaccineTracking;
using ClassLib.Models;
using ClassLib.Repositories;

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
            return vaccinesTrackings.Select(vt => new VaccinesTrackingResponse
            {
                VaccineName = vt.Vaccine.Name,
                UserName = vt.User.Name,
                ChildName = vt.Child.Name,
                MinimumIntervalDate = vt.MinimumIntervalDate,
                VaccinationDate = vt.VaccinationDate,
                MaximumIntervalDate = vt.MaximumIntervalDate,
                Status = vt.Status,
                AdministeredByDoctorName = vt.User.Name,
                Reaction = vt.Reaction
            }).ToList();
        }

        public async Task<List<VaccinesTrackingResponse>> GetVaccinesTrackingByParentIdAsync(int id)
        {
            var vaccinesTrackings = await _vaccinesTrackingRepository.GetVaccinesTrackingByParentIdAsync(id);
            return vaccinesTrackings.Select(vt => new VaccinesTrackingResponse
            {
                VaccineName = vt.Vaccine.Name,
                UserName = vt.User.Name,
                ChildName = vt.Child.Name,
                MinimumIntervalDate = vt.MinimumIntervalDate,
                VaccinationDate = vt.VaccinationDate,
                MaximumIntervalDate = vt.MaximumIntervalDate,
                Status = vt.Status,
                AdministeredByDoctorName = vt.User.Name,
                Reaction = vt.Reaction
            }).ToList();
        }

        public async Task<VaccinesTrackingResponse> GetVaccinesTrackingByIdAsync(int id)
        {
            var vt = await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(id);
            return new VaccinesTrackingResponse
            {
                VaccineName = vt.Vaccine.Name,
                UserName = vt.User.Name,
                ChildName = vt.Child.Name,
                MinimumIntervalDate = vt.MinimumIntervalDate,
                VaccinationDate = vt.VaccinationDate,
                MaximumIntervalDate = vt.MaximumIntervalDate,
                Status = vt.Status,
                AdministeredByDoctorName = vt.User.Name,
                Reaction = vt.Reaction
            };
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
            int previousVaccination = 0;

            foreach (var childID in child)
            {
                foreach (var vaccinesID in vaccines)
                {
                    var vaccine = await _vaccineRepository.GetById(vaccinesID);

                    for (int dosesTimes = 1; dosesTimes <= vaccine!.DoesTimes; dosesTimes++)
                    {
                        if (dosesTimes == 1)  // Fix: First dose should be dosesTimes == 1
                        {
                            var vt = new VaccinesTracking
                            {
                                VaccineId = vaccinesID,
                                UserId = request.UserId,
                                ChildId = childID,
                                VaccinationDate = request.VaccinationDate,
                                Status = "Waiting",
                                AdministeredBy = request.AdministeredBy,
                                MinimumIntervalDate = request.VaccinationDate!.Value.AddDays(2),
                                MaximumIntervalDate = request.VaccinationDate!.Value.AddDays(7),
                                Reaction = "Nothing",
                                PreviousVaccination = previousVaccination
                            };

                            await _vaccinesTrackingRepository.AddVaccinesTrackingAsync(vt);
                            previousVaccination = vt.Id; // Store the first dose's ID
                        }
                        else
                        {
                            // Fix: Ensure previousVaccination is valid
                            if (previousVaccination == 0)
                            {
                                throw new Exception("Error: No valid previous vaccination record found.");
                            }

                            var previousTracking = await _vaccinesTrackingRepository.GetVaccinesTrackingByIdAsync(previousVaccination);
                            if (previousTracking == null )
                            {
                                throw new Exception($"Error: Previous vaccination record (ID: {previousVaccination}) is invalid.");
                            }

                            var vt = new VaccinesTracking
                            {   
                                VaccineId = vaccinesID,
                                UserId = request.UserId,
                                ChildId = childID,
                                VaccinationDate = null,
                                Status = "Waiting",
                                AdministeredBy = request.AdministeredBy,
                                MinimumIntervalDate = previousTracking.MinimumIntervalDate!.AddDays(vaccine.MaximumIntervalDate!.Value),
                                MaximumIntervalDate = null,
                                Reaction = "Nothing",
                                PreviousVaccination = previousVaccination
                            };

                            await _vaccinesTrackingRepository.AddVaccinesTrackingAsync(vt);
                            previousVaccination = vt.Id; // Store current dose ID for next iteration
                        }
                    }

                    previousVaccination = 0; // Reset for next vaccine
                }
            }
            return true;
        }
    }
}