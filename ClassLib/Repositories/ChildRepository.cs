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
        private readonly DbSwpVaccineTrackingFinalContext _context;
        public ChildRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context;
        }

        public async Task<List<Child>> GetAll()
        {
            return await _context.Children.Where(c => c.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Child>> GetAllForAdmin()
        {
            return await _context.Children.ToListAsync();
        }

        public async Task<Child?> GetChildById(int id)
        {
            return await _context.Children.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
        }

        public async Task<Child?> GetChildByIdHardDelete(int id)
        {
            return await _context.Children.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Child>> getAllChildByParentsId(int parentId)
        {
            return await _context.Children.Where(c => c.ParentId == parentId && c.IsDeleted == false).ToListAsync();
        }

        public async Task<bool> CreateChild(Child child)
        {
            await _context.Children.AddAsync(child);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateChild(Child child)
        {
            _context.Children.Update(child);
            return await _context.SaveChangesAsync() > 0;
            
        }

        public async Task<bool> HardDeleteChild(Child child)
        {
            //var child = await GetChildById(id);
            //if (child == null)
            //{
            //    return false;
            //}
            _context.Children.Remove(child);
            return await _context.SaveChangesAsync() > 0;
        }


    }
}
