using Gamess.Core.Entities;
using Gamess.Infraestructure.Data;
using Dapper;

namespace Gamess.Infraestructure.Repositories
{
    public class GameDapperRepository : IGameDapperRepository
    {
        private readonly IDapperContext _ctx;
        public GameDapperRepository(IDapperContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Game>> GetLatestAsync(int take)
        {
            var sql = @"SELECT g.* 
                        FROM Game g
                        ORDER BY g.Id DESC
                        LIMIT @take;";
            return await _ctx.QueryAsync<Game>(sql, new { take });
        }

        public async Task<IEnumerable<Game>> SearchAsync(string title)
        {
            var sql = @"SELECT g.* 
                        FROM Game g
                        WHERE LOWER(g.Title) LIKE CONCAT('%', LOWER(@title), '%');";
            return await _ctx.QueryAsync<Game>(sql, new { title });
        }

        public async Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> TopRatedAsync(int take)
        {
            var sql = @"
        SELECT g.*, agg.AvgScore, agg.ReviewsCount
        FROM (
            SELECT r.GameId, AVG(r.Score) AS AvgScore, COUNT(*) AS ReviewsCount
            FROM Review r
            GROUP BY r.GameId
        ) agg
        JOIN Game g ON g.Id = agg.GameId
        ORDER BY agg.AvgScore DESC, agg.ReviewsCount DESC
        LIMIT @take;";

            var rows = await _ctx.QueryAsync<dynamic>(sql, new { take });
            return rows.Select(r => (
                new Game
                {
                    Id = (int)r.Id,
                    UploaderUserId = (int)r.UploaderUserId,
                    Title = (string)r.Title,
                    Genre = (string)r.Genre,
                    ReleaseDate = r.ReleaseDate as DateTime?,
                    AgeRating = r.AgeRating as string,
                    MinAge = r.MinAge as int?,
                    CoverUrl = r.CoverUrl as string,
                    IsActive = Convert.ToBoolean(r.IsActive)
                },
                AvgScore: (double)r.AvgScore,
                ReviewsCount: (int)r.ReviewsCount
            ));
        }

        public async Task<IEnumerable<(Game Game, double AvgScore, int ReviewsCount)>> LowRatedAsync(int take)
        {
            var sql = @"
        SELECT g.*, agg.AvgScore, agg.ReviewsCount
        FROM (
            SELECT r.GameId, AVG(r.Score) AS AvgScore, COUNT(*) AS ReviewsCount
            FROM Review r
            GROUP BY r.GameId
        ) agg
        JOIN Game g ON g.Id = agg.GameId
        ORDER BY agg.AvgScore ASC, agg.ReviewsCount DESC
        LIMIT @take;";

            var rows = await _ctx.QueryAsync<dynamic>(sql, new { take });
            return rows.Select(r => (
                new Game
                {
                    Id = (int)r.Id,
                    UploaderUserId = (int)r.UploaderUserId,
                    Title = (string)r.Title,
                    Genre = (string)r.Genre,
                    ReleaseDate = r.ReleaseDate as DateTime?,
                    AgeRating = r.AgeRating as string,
                    MinAge = r.MinAge as int?,
                    CoverUrl = r.CoverUrl as string,
                    IsActive = Convert.ToBoolean(r.IsActive)
                },
                AvgScore: (double)r.AvgScore,
                ReviewsCount: (int)r.ReviewsCount
            ));
        }
    }
}
