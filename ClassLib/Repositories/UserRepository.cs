using ClassLib.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLib.Repositories
{
    public class UserRepository
    {
        private readonly DbSwpVaccineTrackingFinalContext _context;
        public UserRepository(DbSwpVaccineTrackingFinalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<User>> getAll()
        {
            //return await _context.Users.ToListAsync();
            return await _context.Users
                        //.Include(u => u.Bookings)
                        //.Include(u => u.Children)
                        //.Include(u => u.RefreshTokens)
                        //.Include(u => u.VaccinesTrackings)
                        .ToListAsync();
        }

        public async Task<User?> getUserByUsernameAsync(string Username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);
        }

        public async Task<User?> getUserByPhoneAsync(string PhoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == PhoneNumber);
        }

        public async Task<User?> getUserByGmailAsync(string gmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Gmail == gmail);
        }

        public async Task<User?> getUserByIdAsync(int Id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == Id);
        }

        public async Task<bool> addUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task addRefreshToken(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> getRefreshTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.RefreshToken1 == refreshToken);
        }

        public async Task updateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> getRefreshTokenByUserId(int? userId)
        {
            return await _context.RefreshTokens.Where(r => r.UserId == userId && !r.IsUsed && !r.IsRevoked && r.ExpiredAt > DateTime.UtcNow)
                                               .OrderByDescending(r => r.IssuedAt)
                                               .FirstOrDefaultAsync(); ;
        }

        public async Task<bool> updateUser(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> deleteUser(User user)
        {
            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
