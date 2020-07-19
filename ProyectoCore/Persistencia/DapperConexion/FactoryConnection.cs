using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Persistencia.DapperConexion
{
    public class FactoryConnection : IFactoryConnection
    {
        private  IDbConnection connection;

        private readonly IOptions<ConexionConfiguracion> configs;
        public FactoryConnection(IOptions<ConexionConfiguracion> _configs)
        {
            configs=_configs;
        }
        public void CloseConnection()
        {
            if(connection!=null && connection.State ==ConnectionState.Open){
                connection.Close();
            }
        }

        public IDbConnection GetConnection()
        {
            if(connection==null){
                connection= new SqlConnection(configs.Value.DefaultConnection); 
            }
            if(connection.State != ConnectionState.Open){
                connection.Open();
            }
            return connection;
        }
    }
}