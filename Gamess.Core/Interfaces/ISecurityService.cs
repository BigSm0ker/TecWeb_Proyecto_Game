using Gamess.Core.Entities;

namespace Gamess.Core.Interfaces
{
    public interface ISecurityService
    {
        Task<Security?> GetLoginByCredentials(UserLogin login);
        Task RegisterUser(Security security);
    }
}