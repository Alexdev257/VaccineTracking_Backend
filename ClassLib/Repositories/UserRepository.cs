using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLib.DTO.User;
using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class UserRepository
    {
        private readonly DbSwpVaccineTracking2Context _context;
        public UserRepository(DbSwpVaccineTracking2Context context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<User>> getAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> getUserByUsernameAsync(string Username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);
        }

        public async Task<User?> getUserByPhoneAsync(string PhoneNumber)
        {
            // ngăn lỗi giữ lại dbcontext
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == PhoneNumber);
        }

        public async Task addUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

    }
}
