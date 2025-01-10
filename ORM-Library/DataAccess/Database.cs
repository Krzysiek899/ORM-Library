using System.Data;
using System.Data.Common;

public class Database
{

    private static Database? _instance;
    private DbConnection _connection;

    private Database(DbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    public static Database Instance(DbConnection connection)
    {
        if (_instance == null)
        {
            _instance = new Database(connection);
        }
        else {
            _instance.ChangeConnection(connection);
        }
        return _instance;
    }
    
    public bool Connect(){
        try {
            _connection.Open();
            return true;
        }
        catch (Exception e){
            return false;
        }
    }

    public bool Disconnect(){
        try {
            if (_connection.State != ConnectionState.Closed) {
                _connection.Close();
            }
            return true;
        }
        catch (Exception e){
            return false;
        }
    }

    public void ChangeConnection(DbConnection newConnection){
        if (_connection.State == ConnectionState.Open) {
            Disconnect();
        }
        new Database(newConnection);
    }
    
}