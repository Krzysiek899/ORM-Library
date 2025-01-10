using System;
using System.Data;
using System.Data.Common;
public abstract class DataBaseContext
{
    private DatabaseConnection _databaseConnection;
    private ModelBuilder _modelBuilder;

    public DataBaseContext (string connectionString, IDbConnectionFactory factory){

        _databaseConnection = new DatabaseConnection(connectionString, factory);
        _modelBuilder = new ModelBuilder();

    }

    public Table<T> Set<T>() where T : class
    {
        // Zwróć DbSet odpowiedni do typu T
        return new Table<T>(_databaseConnection);
    }

    protected abstract void OnModelCreating(ModelBuilder modelBuilder);

    public bool MapToDatabase(){
        
        



        return true;
    }


}