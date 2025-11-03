using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task InsertAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
        Task<PagedList<User>> GetAllFilteredAsync(UserQueryFilter filters, PaginationQueryFilter pagination);
    }
}
