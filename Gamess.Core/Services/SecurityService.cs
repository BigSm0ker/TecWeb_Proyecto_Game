using Gamess.Core.Entities;
using Gamess.Core.Interfaces;

namespace Gamess.Core.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SecurityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Security?> GetLoginByCredentials(UserLogin login)
        {
            
            return null;
        }

        public async Task RegisterUser(Security security)
        {
         
        }
    }
}