using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class ChildRepository
    {
        private readonly DbSwpVaccineTrackingContext _context;
        public ChildRepository(DbSwpVaccineTrackingContext context)
        {
            _context = context;
        }

        public async Task<List<Child>> GetAll()
        {
            return await _context.Children.ToListAsync();
        }

        public async Task<Child?> GetById(int id)
        {
            return await _context.Children.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Child>> getAllChildByParentsId(int parentId)
        {
            return await _context.Children.Where(c => c.ParentId == parentId).ToListAsync();
        }


    }
}
