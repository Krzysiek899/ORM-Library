using System.Data.Common;
using Npgsql;

namespace ORMLibrary.DataAccess.DatabaseConnectionFactories
{
    public class PostgreSqlConnectionFactory : IDbConnectionFactory
    {
        public DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}