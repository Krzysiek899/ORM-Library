using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

public class InsertCommand : ICommand 
{
    // Prywatne pola przechowujące referencje do obiektu Database, zapytania SQL i parametrów
    // private readonly Database _database;
    // private readonly string _query;
    // private readonly DbParameter[] _parameters;

    // // Konstruktor klasy InsertCommand, który inicjalizuje pola
    // public InsertCommand(Database database, string query, DbParameter[] parameters)
    // {
    //     _database = database; // Przypisanie obiektu Database
    //     _query = query; // Przypisanie zapytania SQL
    //     _parameters = parameters; // Przypisanie parametrów
    // }

    // // Metoda Execute, która wykonuje zapytanie SQL
    // public void Execute()
    // {
    //     // Tworzenie obiektu DbCommand przy użyciu połączenia z bazy danych
    //     using (var command = _database.Connection.CreateCommand())
    //     {
    //         // Ustawienie tekstu zapytania SQL
    //         command.CommandText = _query;
    //         // Ustawienie typu polecenia na tekstowe
    //         command.CommandType = CommandType.Text;
    //         // Dodanie parametrów do polecenia
    //         command.Parameters.AddRange(_parameters);
    //         // Otwarcie połączenia z bazą danych
    //         _database.Connect();
    //         // Wykonanie polecenia SQL
    //         command.ExecuteNonQuery();
    //         // Zamknięcie połączenia z bazą danych
    //         _database.Disconnect();
    //     }
    // }
}