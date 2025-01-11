using System.Data.Common;
using System.Data.SqlClient;

namespace ORMLibrary.DataAccess.DatabaseConnectionFactories
{
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}