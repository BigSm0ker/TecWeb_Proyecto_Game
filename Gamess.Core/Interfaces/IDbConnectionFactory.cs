using System.Data;

namespace Gamess.Infraestructure.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
