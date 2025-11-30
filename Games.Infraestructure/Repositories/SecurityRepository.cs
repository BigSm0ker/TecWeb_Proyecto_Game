using Microsoft.EntityFrameworkCore;
using Gamess.Core.Entities;
using Gamess.Core.Interfaces;
using Games.Infrastructure.Data;

namespace Games.Infrastructure.Repositories
{
    public class SecurityRepository : BaseRepository<Security>, ISecurityRepository
    {
        public SecurityRepository(GamesContext context) : base(context)
        {
        }

        public async Task<Security?> GetLoginByCredentials(UserLogin login)
        {
            return await _set.FirstOrDefaultAsync(x => x.Login == login.User);
        }
    }
}