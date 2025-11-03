using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Interfaces
{
    public interface IGameService
    {
        Task<IEnumerable<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(int id);
        Task<Game> CreateAsync(Game game);
        Task<Game> UpdateAsync(Game game);
        Task DeleteAsync(int id);

        Task<IEnumerable<Game>> GetByGenreAsync(string genre);
        Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetTopRatedAsync(int take);
        Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetLowRatedAsync(int take);
        Task<IEnumerable<Game>> SearchByTitleAsync(string title);
        Task<IEnumerable<Game>> GetByAgeRangeAsync(int? min, int? max, bool includeUnknown);
        Task<PagedList<Game>> GetAllAsync(GameQueryFilter filters, PaginationQueryFilter pagination);

    }
}

