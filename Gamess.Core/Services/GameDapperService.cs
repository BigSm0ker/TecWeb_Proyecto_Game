using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Gamess.Infraestructure.Repositories;

namespace Gamess.Core.Services
{
    public class GameDapperService : IGameDapperService
    {
        private readonly IGameDapperRepository _repo;
        public GameDapperService(IGameDapperRepository repo) => _repo = repo;

        public Task<IEnumerable<Game>> GetLatestAsync(int take) => _repo.GetLatestAsync(take);
        public Task<IEnumerable<Game>> SearchAsync(string title) => _repo.SearchAsync(title);
        public Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> TopRatedAsync(int take) => _repo.TopRatedAsync(take);
        public Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> LowRatedAsync(int take) => _repo.LowRatedAsync(take);
    }
}
