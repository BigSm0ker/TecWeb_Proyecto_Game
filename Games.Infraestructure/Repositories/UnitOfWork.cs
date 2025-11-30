using Gamess.Core.Interfaces;
using Games.Infrastructure.Data;

namespace Games.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly GamesContext _ctx;

        public IUserRepository Users { get; }
        public IGameRepository Games { get; }
        public IReviewRepository Reviews { get; }
        public ISecurityRepository Securities { get; }

        public UnitOfWork(
            GamesContext ctx,
            IUserRepository users,
            IGameRepository games,
            IReviewRepository reviews,
            ISecurityRepository securities)
        {
            _ctx = ctx;
            Users = users;
            Games = games;
            Reviews = reviews;
            Securities = securities;
        }

        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
            => _ctx.SaveChangesAsync(cancellationToken);

        public void Dispose() => _ctx.Dispose();
    }
}
