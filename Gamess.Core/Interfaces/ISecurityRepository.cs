using Gamess.Core.Entities;

namespace Gamess.Core.Interfaces
{
    public interface ISecurityRepository : IBaseRepository<Security>
    {
        Task<Security?> GetLoginByCredentials(UserLogin login);
    }
}