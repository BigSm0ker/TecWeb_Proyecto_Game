using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Gamess.Core.QueryFilters;
using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Games.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly GamesContext _ctx;
        public ReviewRepository(GamesContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Review>> GetByGameAsync(int gameId) =>
            await _ctx.Reviews
                .Include(r => r.User)
                .Where(r => r.GameId == gameId)
                .ToListAsync();

        public async Task<Review?> GetByIdAsync(int id) =>
            await _ctx.Reviews.Include(r => r.User).Include(r => r.Game)
                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task InsertAsync(Review review)
        {
            _ctx.Reviews.Add(review);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _ctx.Reviews.Update(review);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Review review)
        {
            _ctx.Reviews.Remove(review);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetAllAsync() =>
            await _ctx.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .ToListAsync();

        public async Task<PagedList<Review>> GetAllFilteredAsync(ReviewQueryFilter filters, PaginationQueryFilter pagination)
        {
            var q = _ctx.Reviews
                .Include(r => r.Game)
                .Include(r => r.User)
                .AsQueryable();

            if (filters?.GameId is not null)
                q = q.Where(r => r.GameId == filters.GameId.Value);

            if (filters?.UserId is not null)
                q = q.Where(r => r.UserId == filters.UserId.Value);

            if (filters?.MinScore is not null)
                q = q.Where(r => r.Score >= filters.MinScore.Value);

            if (filters?.MaxScore is not null)
                q = q.Where(r => r.Score <= filters.MaxScore.Value);

            if (filters?.From is not null)
                q = q.Where(r => r.CreatedAt >= filters.From.Value);

            if (filters?.To is not null)
                q = q.Where(r => r.CreatedAt <= filters.To.Value);

            if (filters?.IsActive is not null)
                q = q.Where(r => r.IsActive == filters.IsActive.Value);

            q = q.OrderByDescending(r => r.CreatedAt).ThenBy(r => r.Id);

            var count = await q.CountAsync();
            var items = await q.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                               .Take(pagination.PageSize)
                               .ToListAsync();

            return new PagedList<Review>(items, count, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<PagedList<Review>> GetByGameFilteredAsync(int gameId, ReviewQueryFilter filters, PaginationQueryFilter pagination)
        {
            filters ??= new ReviewQueryFilter();
            filters.GameId = gameId;
            return await GetAllFilteredAsync(filters, pagination);
        }

    }
}
