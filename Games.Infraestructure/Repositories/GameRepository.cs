using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Gamess.Core.Entities;
using Gamess.Core.Interfaces;

using Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Games.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GamesContext _ctx;
        public GameRepository(GamesContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Game>> GetAllAsync() =>
            await _ctx.Games
                .Include(g => g.Uploader)
                .Include(g => g.Reviews)
                .ToListAsync();

        public async Task<Game?> GetByIdAsync(int id) =>
            await _ctx.Games
                .Include(g => g.Uploader)
                .Include(g => g.Reviews)
                .FirstOrDefaultAsync(g => g.Id == id);

        public async Task InsertAsync(Game game)
        {
            _ctx.Games.Add(game);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Game game)
        {
            _ctx.Games.Update(game);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(Game game)
        {
            _ctx.Games.Remove(game);
            await _ctx.SaveChangesAsync();
        }
        public async Task<IEnumerable<Game>> GetByGenreAsync(string genre)
        {
           
            return await _ctx.Games
                .Include(g => g.Reviews)
                .Where(g => g.Genre != null &&
                            EF.Functions.Like(g.Genre, genre)) 
                .ToListAsync();
           
        }

        public async Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetTopRatedAsync(int take)
        {
            
            var query = _ctx.Reviews
                .GroupBy(r => r.GameId)
                .Select(g => new
                {
                    GameId = g.Key,
                    Avg = g.Average(r => r.Score),
                    Cnt = g.Count()
                })
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Cnt)
                .Take(take);

            
            var joined = from q in query
                         join game in _ctx.Games.Include(g => g.Reviews)
                             on q.GameId equals game.Id
                         select new { game, q.Avg, q.Cnt };

            var list = await joined.ToListAsync();
            return list.Select(x => (x.game, x.Avg, x.Cnt));
        }
        public async Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetLowRatedAsync(int take)
        {
            var query = _ctx.Reviews
                .GroupBy(r => r.GameId)
                .Select(g => new
                {
                    GameId = g.Key,
                    Avg = g.Average(r => r.Score),
                    Cnt = g.Count()
                })
                .OrderBy(x => x.Avg)
                .ThenByDescending(x => x.Cnt)
                .Take(take);

            var joined = from q in query
                         join game in _ctx.Games.Include(g => g.Reviews)
                             on q.GameId equals game.Id
                         select new { game, q.Avg, q.Cnt };

            var list = await joined.ToListAsync();
            return list.Select(x => (x.game, x.Avg, x.Cnt));
        }
        public async Task<IEnumerable<Game>> SearchByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<Game>();

           
            return await _ctx.Games
                .Include(g => g.Reviews)
                .Where(g => g.Title.ToLower().Contains(title.ToLower()))
                .ToListAsync();
        }
        public async Task<IEnumerable<Game>> GetByAgeRangeAsync(int? min, int? max, bool includeUnknown = false)
        {
            var q = _ctx.Games
                .Include(g => g.Reviews)
                .AsQueryable();

            
            if (!includeUnknown)
                q = q.Where(g => g.MinAge != null);

            if (min.HasValue)
                q = q.Where(g => g.MinAge >= min.Value);

            if (max.HasValue)
                q = q.Where(g => g.MinAge <= max.Value);

            
            q = q.OrderBy(g => g.MinAge)
                 .ThenByDescending(g => g.Reviews.Count == 0 ? 0 : g.Reviews.Average(r => r.Score));

            return await q.ToListAsync();
        }



    }
}

