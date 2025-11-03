using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Gamess.Core.QueryFilters;
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

        public async Task<PagedList<User>> GetAllFilteredAsync(UserQueryFilter filters, PaginationQueryFilter pagination)
        {
            var q = _ctx.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters?.FirstName))
                q = q.Where(u => u.FirstName != null &&
                                 EF.Functions.Like(u.FirstName.ToLower(), $"%{filters.FirstName.ToLower()}%"));

            if (!string.IsNullOrWhiteSpace(filters?.LastName))
                q = q.Where(u => u.LastName != null &&
                                 EF.Functions.Like(u.LastName.ToLower(), $"%{filters.LastName.ToLower()}%"));

            if (!string.IsNullOrWhiteSpace(filters?.Email))
                q = q.Where(u => u.Email != null &&
                                 EF.Functions.Like(u.Email.ToLower(), $"%{filters.Email.ToLower()}%"));

            if (filters?.IsActive is not null)
                q = q.Where(u => u.IsActive == filters.IsActive.Value);

            q = q.OrderBy(u => u.Id);

            var count = await q.CountAsync();
            var items = await q.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                               .Take(pagination.PageSize)
                               .ToListAsync();

            return new PagedList<User>(items, count, pagination.PageNumber, pagination.PageSize);
        }

    }
}
