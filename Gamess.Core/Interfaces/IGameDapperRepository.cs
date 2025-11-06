using Gamess.Core.Entities;

namespace Gamess.Infraestructure.Repositories
{
    public interface IGameDapperRepository
    {
        Task<IEnumerable<Game>> GetLatestAsync(int take);
        Task<IEnumerable<Game>> SearchAsync(string title);
        Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> TopRatedAsync(int take);
        Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> LowRatedAsync(int take);
    }
}
