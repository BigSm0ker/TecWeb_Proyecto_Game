using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.Exceptions;
using Gamess.Core.Interfaces;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _uow;
        public ReviewService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<Review>> GetByGameAsync(int gameId) => _uow.Reviews.GetByGameAsync(gameId);
        public Task<IEnumerable<Review>> GetAllAsync() => _uow.Reviews.GetAllAsync();
        public Task<Review?> GetByIdAsync(int id) => _uow.Reviews.GetByIdAsync(id);

        public async Task<Review> CreateAsync(Review review)
        {
            await _uow.Reviews.InsertAsync(review);
            await _uow.CommitAsync();
            return review;
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            var existing = await _uow.Reviews.GetByIdAsync(review.Id);
            if (existing is null)
                throw new NotFoundException($"Review {review.Id} no existe.");

            await _uow.Reviews.UpdateAsync(review);
            await _uow.CommitAsync();
            return review;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _uow.Reviews.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException($"Review {id} no existe.");

            await _uow.Reviews.DeleteAsync(existing);
            await _uow.CommitAsync();
        }

        
        public Task<PagedList<Review>> GetAllAsync(ReviewQueryFilter filters, PaginationQueryFilter pagination)
            => _uow.Reviews.GetAllFilteredAsync(filters, pagination);

        public Task<PagedList<Review>> GetByGameAsync(int gameId, ReviewQueryFilter filters, PaginationQueryFilter pagination)
            => _uow.Reviews.GetByGameFilteredAsync(gameId, filters, pagination);

        
        public Task<PagedList<Review>> GetByGameFilteredAsync(int gameId, ReviewQueryFilter filters, PaginationQueryFilter pagination)
            => _uow.Reviews.GetByGameFilteredAsync(gameId, filters, pagination);
    }
}