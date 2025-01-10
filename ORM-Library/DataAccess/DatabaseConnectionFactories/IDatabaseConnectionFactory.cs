using System.Data.Common;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection(string connectionString);
}