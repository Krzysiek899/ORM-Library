using System.Data.Common;
using Npgsql;

public class PostgreSqlConnectionFactory : IDbConnectionFactory
{
    public DbConnection CreateConnection(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
    }
}