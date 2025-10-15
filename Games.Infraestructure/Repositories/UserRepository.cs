using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly GamesContext _ctx;
        public UserRepository(GamesContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _ctx.Users.AsNoTracking().ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _ctx.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task InsertAsync(User user)
        {
            _ctx.Users.Add(user);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _ctx.Users.Update(user);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _ctx.Users.Remove(user);
            await _ctx.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(int id) =>
            _ctx.Users.AnyAsync(u => u.Id == id);

        public Task<bool> EmailExistsAsync(string email, int? excludeId = null) =>
            _ctx.Users.AnyAsync(u => u.Email == email && (excludeId == null || u.Id != excludeId));
    }
}
