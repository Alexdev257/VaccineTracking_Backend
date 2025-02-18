using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service.Vaccines
{
    public class VaccineService
    {
        private readonly VaccineRepository _vaccineRepository;

        public VaccineService(VaccineRepository vaccineRepository)
        {
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
        }

        public async Task<List<Vaccine>> GetAllVaccines()
        {
            var vaccines = await _vaccineRepository.GetAllVaccines();
            return vaccines.Select(v => new Vaccine
            {
                //Id = v.Id,
                //Name = v.Name,
                //Quantity = v.Quantity,
                //Description = v.Description,
                //Image = v.Image,
                //Price = v.Price,
                //DoesTimes = v.DoesTimes,
                //SuggestAgeMin = v.SuggestAgeMin,
                //SuggestAgeMax = v.SuggestAgeMax,
                //EntryDate = v.EntryDate,
                //TimeExpired = v.TimeExpired,
                //AddressId = v.AddressId,
                //Status = v.Status,
                //MaxiumIntervalDate = v.MaxiumIntervalDate,
                //MiniumIntervalDate = v.MiniumIntervalDate,
                //Country = v.Country,
                //Address = v.Address
            }).ToList();
        }
    }
}
