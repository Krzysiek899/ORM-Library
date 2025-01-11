using System.Data.Common;
using MySql.Data.MySqlClient;

namespace ORMLibrary.DataAccess.DatabaseConnectionFactories
{

    public class MySqlConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}