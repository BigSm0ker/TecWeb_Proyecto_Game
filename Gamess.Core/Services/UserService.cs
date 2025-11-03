using Gamess.Core.CustomEntities;
using Gamess.Core.Entities;
using Gamess.Core.Exceptions;
using Gamess.Core.Interfaces;
using Gamess.Core.QueryFilters;

namespace Gamess.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;
        public UserService(IUnitOfWork uow) => _uow = uow;

        public Task<IEnumerable<User>> GetAllAsync() => _uow.Users.GetAllAsync();
        public Task<User?> GetByIdAsync(int id) => _uow.Users.GetByIdAsync(id);

        public async Task<User> CreateAsync(User user)
        {
            if (await _uow.Users.EmailExistsAsync(user.Email))
                throw new ConflictException("Email ya está registrado.");

            await _uow.Users.InsertAsync(user);
            await _uow.CommitAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existing = await _uow.Users.GetByIdAsync(user.Id);
            if (existing is null)
                throw new NotFoundException($"User {user.Id} no existe.");

            if (await _uow.Users.EmailExistsAsync(user.Email, user.Id))
                throw new ConflictException("Email ya está registrado por otro usuario.");

            await _uow.Users.UpdateAsync(user);
            await _uow.CommitAsync();
            return user;
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _uow.Users.GetByIdAsync(id);
            if (existing is null)
                throw new NotFoundException($"User {id} no existe.");

            await _uow.Users.DeleteAsync(existing);
            await _uow.CommitAsync();
        }

        // NUEVO
        public Task<PagedList<User>> GetAllAsync(UserQueryFilter filters, PaginationQueryFilter pagination)
            => _uow.Users.GetAllFilteredAsync(filters, pagination);
    }
}
