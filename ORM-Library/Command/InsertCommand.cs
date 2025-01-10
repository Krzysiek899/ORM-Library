using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient; // Dodanie tej dyrektywy using pozwala na korzystanie z klasy SqlParameter
public class InsertCommand : ICommand 
{
    private readonly Database _database;
    private readonly string _query;
    private readonly DbParameter[] _parameters;

    public InsertCommand(Database database, string query, DbParameter[] parameters)
    {
        _database = database;
        _query = query;
        _parameters = parameters;
    }

    public void Execute()
    {
        using (var command = _database.Connection.CreateCommand())
        {
            command.CommandText = _query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddRange(_parameters);
            _database.Connect();
            command.ExecuteNonQuery();
            _database.Disconnect();
        }
    }

}

