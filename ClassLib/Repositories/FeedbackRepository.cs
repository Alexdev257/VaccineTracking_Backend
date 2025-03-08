using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<List<Feedback>> getFeedBackById(int id)
        {
            return await _context.Feedbacks.Where(f => f.Id == id).ToListAsync();
        }

        public async Task<bool> addFeedback(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> updateFeedBack(int id, Feedback feedback)
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
