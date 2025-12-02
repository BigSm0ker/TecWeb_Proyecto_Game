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
            return await _unitOfWork.Securities.GetLoginByCredentials(login);
        }

        public async Task RegisterUser(Security security)
        {
            

            await _unitOfWork.Securities.AddAsync(security);
            await _unitOfWork.CommitAsync();
        }
    }
}