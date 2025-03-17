using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class FeedbackRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;

        public FeedbackRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context;
        }
        
        public async Task<List<Feedback>> getAllFeedBackRepo()
        {
            return await _context.Feedbacks.Where(f => f.IsDeleted == false).ToListAsync();
        }

        public async Task<Feedback?> getFeedBackById(int id)
        {
            return await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id && f.IsDeleted == false);
        }

        public async Task<List<Feedback>> getFeedBackByUserId(int userId)
        {
            return await _context.Feedbacks.Where(f => f.UserId == userId && f.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Feedback>> getAllFeedBackRepoAdmin()
        {
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<Feedback?> getFeedBackByIdAdmin(int id)
        {
            return await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Feedback>> getFeedBackByUserIdAdmin(int userId)
        {
            return await _context.Feedbacks.Where(f => f.UserId == userId).ToListAsync();
        }

        public async Task<bool> addFeedback(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> updateFeedBack(Feedback feedback)
        {
             _context.Feedbacks.Update(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> hardDeleteFeedback(Feedback feedback)
        {
            _context.Feedbacks.Remove(feedback);
            return await _context.SaveChangesAsync() > 0;
        }


        //public async Task<bool> updateUser(User user)
        //{
        //    _context.Users.Update(user);
        //    return await _context.SaveChangesAsync() > 0;
        //}
    }
}
