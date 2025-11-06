using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Gamess.Infraestructure.Data
{
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnectionFactory _factory;

        public DapperContext(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IDbConnection> OpenIfNeededAsync(IDbConnection? conn = null)
        {
            var c = conn ?? _factory.CreateConnection();
            if (c.State != ConnectionState.Open)
                await ((dynamic)c).OpenAsync(); // MySqlConnection tiene OpenAsync
            return c;
        }

        public async Task CloseDisposeAsync(IDbConnection conn)
        {
            if (conn != null)
            {
                if (conn.State != ConnectionState.Closed)
                    await ((dynamic)conn).CloseAsync();
                conn.Dispose();
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var conn = _factory.CreateConnection();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryAsync<T>(sql, param, commandType: commandType);
            }
            finally
            {
                await CloseDisposeAsync(conn);
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var conn = _factory.CreateConnection();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryFirstOrDefaultAsync<T>(sql, param, commandType: commandType);
            }
            finally
            {
                await CloseDisposeAsync(conn);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            var conn = _factory.CreateConnection();
            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.ExecuteAsync(sql, param, commandType: commandType);
            }
            finally
            {
                await CloseDisposeAsync(conn);
            }
        }
    }
}
