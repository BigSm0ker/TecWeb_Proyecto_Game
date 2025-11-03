using System.Linq;
using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.Exceptions;
using Gamess.Core.Interfaces;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _uow;

        private static readonly string[] ForbiddenWordsInTitle = { "hack", "cheat", "pirata" };
        private const int BadScoreThreshold = 4;
        private const int BadCountLimit = 5;

        public GameService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<Game>> GetAllAsync() => _uow.Games.GetAllAsync();

        public async Task<Game?> GetByIdAsync(int id) => await _uow.Games.GetByIdAsync(id);

        public async Task<Game> CreateAsync(Game game)
        {
            ValidateTitleOrThrow(game.Title);
            await _uow.Games.InsertAsync(game);
            await _uow.CommitAsync();
            return game;
        }

        public async Task<Game> UpdateAsync(Game game)
        {
            var existing = await _uow.Games.GetByIdAsync(game.Id);
            if (existing is null)
                throw new NotFoundException($"Game {game.Id} no existe.");

            ValidateTitleOrThrow(game.Title);

            await _uow.Games.UpdateAsync(game);
            await _uow.CommitAsync();

            var loaded = await _uow.Games.GetByIdAsync(game.Id);
            if (loaded is not null && loaded.Reviews != null && loaded.Reviews.Count >= BadCountLimit)
            {
                var bads = loaded.Reviews.Count(r => r.Score <= BadScoreThreshold);
                if (bads >= BadCountLimit)
                {
                    loaded.IsActive = false;
                    await _uow.Games.UpdateAsync(loaded);
                    await _uow.CommitAsync();
                }
            }
            return loaded ?? game;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _uow.Games.GetByIdAsync(id);
            if (entity is null)
                throw new NotFoundException($"Game {id} no existe.");

            await _uow.Games.DeleteAsync(entity);
            await _uow.CommitAsync();
        }

        public Task<IEnumerable<Game>> GetByGenreAsync(string genre) => _uow.Games.GetByGenreAsync(genre);
        public Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetTopRatedAsync(int take) => _uow.Games.GetTopRatedAsync(take);
        public Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> GetLowRatedAsync(int take) => _uow.Games.GetLowRatedAsync(take);
        public Task<IEnumerable<Game>> SearchByTitleAsync(string title) => _uow.Games.SearchByTitleAsync(title);
        public Task<IEnumerable<Game>> GetByAgeRangeAsync(int? min, int? max, bool includeUnknown) => _uow.Games.GetByAgeRangeAsync(min, max, includeUnknown);

        // NUEVO: ahora se ejecuta en SQL (IQueryable en el repo)
        public Task<PagedList<Game>> GetAllAsync(GameQueryFilter filters, PaginationQueryFilter pagination)
            => _uow.Games.GetAllFilteredAsync(filters, pagination);

        private void ValidateTitleOrThrow(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BadRequestDomainException("El título es requerido.");

            foreach (var w in ForbiddenWordsInTitle)
                if (title.Contains(w, StringComparison.OrdinalIgnoreCase))
                    throw new BusinessRuleException($"El título contiene una palabra no permitida: '{w}'.");
        }
    }
}
