using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetByGameAsync(int gameId);
        Task<Review?> GetByIdAsync(int id);
        Task<Review> CreateAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(int id);
        Task<PagedList<Review>> GetAllAsync(ReviewQueryFilter filters, PaginationQueryFilter pagination);
        Task<PagedList<Review>> GetByGameFilteredAsync(int gameId, ReviewQueryFilter filters, PaginationQueryFilter pagination); // Added this method
    }
}
