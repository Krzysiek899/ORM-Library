using System.Data.Common;

namespace ORMLibrary.DataAccess.DatabaseConnectionFactories
{
    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection(string connectionString);
    }
}