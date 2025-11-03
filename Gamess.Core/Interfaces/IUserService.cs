using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<PagedList<User>> GetAllAsync(UserQueryFilter filters, PaginationQueryFilter pagination);
    }
}
