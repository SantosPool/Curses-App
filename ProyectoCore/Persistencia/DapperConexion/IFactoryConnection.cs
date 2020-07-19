using System.Data;

namespace Persistencia.DapperConexion
{
    public interface IFactoryConnection
    {
         void CloseConnection();
         IDbConnection GetConnection();
    }
}