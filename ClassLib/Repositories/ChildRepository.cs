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
            return await _context.Children.ToListAsync();
        }

        public async Task<Child?> GetChildById(int id)
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
            _context.Children.Remove(child);
            return await _context.SaveChangesAsync() > 0;
        }


    }
}
