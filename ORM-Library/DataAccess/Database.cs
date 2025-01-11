using System;
using System.Data;
using System.Data.Common;
using ORMLibrary.DataAccess.DatabaseConnectionFactories;

namespace ORMLibrary.DataAccess
{

    public class DatabaseConnection
    {
        private DbConnection _connection;

        // Konstruktor z typem bazy danych
        public DatabaseConnection(string connectionString, IDbConnectionFactory factory)
        {
            _connection = factory.CreateConnection(connectionString);
        }  

        // Metoda do uzyskiwania połączenia
        public DbConnection GetConnection()
        {
            return _connection;
        }

        // Otwarcie połączenia
        public void Open()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        // Zamknięcie połączenia
        public void Close()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public QueryExecutor CreateQueryExecutor()
        {
            return new QueryExecutor(_connection);
        }
    }
}