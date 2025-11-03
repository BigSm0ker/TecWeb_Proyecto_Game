using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Games.Infrastructure.Data;

namespace Games.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly GamesContext _ctx;
        protected readonly DbSet<T> _set;

        public BaseRepository(GamesContext ctx)
        {
            _ctx = ctx;
            _set = _ctx.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() =>
            await _set.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) =>
            await _set.FindAsync(id);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
            await _set.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            // NO SaveChanges aquí (UoW)
        }

        public void Update(T entity)
        {
            _set.Update(entity);
            // NO SaveChanges aquí (UoW)
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
            // NO SaveChanges aquí (UoW)
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) _set.Remove(entity);
            // NO SaveChanges aquí (UoW)
        }
    }
}
