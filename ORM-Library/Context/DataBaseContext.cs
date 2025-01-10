using System;
using System.Data;
using System.Data.Common;
public abstract class DataBaseContext
{
    private DatabaseConnection _databaseConnection;
    private ModelBuilder _modelBuilder;
    private DatabaseMapper _databaseMapper;

    public DataBaseContext (string connectionString, IDbConnectionFactory factory){

        _databaseConnection = new DatabaseConnection(connectionString, factory);
        _modelBuilder = new ModelBuilder();
        _databaseMapper = new DatabaseMapper(_databaseConnection, _modelBuilder);

    }

    public Table<T> Set<T>() where T : class
    {
        // Zwróć DbSet odpowiedni do typu T
        return new Table<T>(_databaseConnection);
    }

    protected abstract void OnModelCreating(ModelBuilder modelBuilder);

    public bool Map(){
        
        _databaseMapper.MapToDatabase();




        return true;
    }


}