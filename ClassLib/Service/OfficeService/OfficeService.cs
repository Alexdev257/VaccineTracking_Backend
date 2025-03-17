using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Models;
using ClassLib.Repositories;

namespace ClassLib.Service.OfficeService
{
    public class OfficeService
    {
        private readonly OfficeRepository _officeRepository;
        public OfficeService(OfficeRepository officeRepository)
        {
            _officeRepository = officeRepository ?? throw new ArgumentNullException(nameof(officeRepository));   
        }

        //Lấy tất cả
        public async Task<List<Office>> GetAllOffice()
        {
            return await _officeRepository.GetAll();
        }
    }
}
