using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gamess.Core.Entities;

using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Games.Infrastructure.Repositories;
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

}
