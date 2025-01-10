using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

public class QueryExecutor
{
    private DbConnection _connection;

    public QueryExecutor(DbConnection connection)
    {
        _connection = connection;
    }

    public int ExecuteNonQuery(string query, List<DbParameter>? parameters = null)
    {
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = query;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }

            // Ensure the connection is open
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            return command.ExecuteNonQuery();
        }
    }

    public DataTable ExecuteQuery(string query, List<DbParameter>? parameters = null)
    {
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = query;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.Add(param);
                }
            }

            // Ensure the connection is open
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            using (var reader = command.ExecuteReader())
            {
                var dataTable = new DataTable();
                dataTable.Load(reader);
                return dataTable;
            }
        }
    }
}
