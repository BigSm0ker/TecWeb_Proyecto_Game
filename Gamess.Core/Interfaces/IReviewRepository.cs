using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gamess.Core.Entities
{

    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetByGameAsync(int gameId);
        Task<Review?> GetByIdAsync(int id);
        Task InsertAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task<IEnumerable<Review>> GetAllAsync();

    }
}
