using System.Threading;
using System.Threading.Tasks;

namespace Gamess.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IGameRepository Games { get; }
        IReviewRepository Reviews { get; }

        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
