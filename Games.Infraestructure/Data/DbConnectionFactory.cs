using System.Data;
using MySqlConnector;
using Microsoft.Extensions.Configuration;

namespace Gamess.Infraestructure.Data
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
       
            _connectionString = configuration.GetConnectionString("ConnectionMySql")!;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
