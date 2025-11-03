using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetByGameAsync(int gameId);
        Task<Review?> GetByIdAsync(int id);
        Task InsertAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task<IEnumerable<Review>> GetAllAsync();
        Task<PagedList<Review>> GetAllFilteredAsync(ReviewQueryFilter filters, PaginationQueryFilter pagination);
        Task<PagedList<Review>> GetByGameFilteredAsync(int gameId, ReviewQueryFilter filters, PaginationQueryFilter pagination);
    }
}
