using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gamess.Core.Entities;


namespace Gamess.Core.Interfaces
{
    public interface IGameRepository
    {
        Task<IEnumerable<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(int id);
        Task InsertAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(Game game);
        Task<IEnumerable<Game>> GetByGenreAsync(string genre);
        Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetTopRatedAsync(int take);
        Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetLowRatedAsync(int take);
        Task<IEnumerable<Game>> SearchByTitleAsync(string title);
        Task<IEnumerable<Game>> GetByAgeRangeAsync(int? min, int? max, bool includeUnknown = false);


    }
}


