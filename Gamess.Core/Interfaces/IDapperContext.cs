using System.Data;
using System.Threading.Tasks;


namespace Gamess.Infraestructure.Data
{
    public interface IDapperContext
    {
        Task<IDbConnection> OpenIfNeededAsync(IDbConnection? conn = null);
        Task CloseDisposeAsync(IDbConnection conn);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text);
    }
}
