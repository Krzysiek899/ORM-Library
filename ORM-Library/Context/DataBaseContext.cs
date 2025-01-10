using System;
using System.Data;
using System.Data.Common;
public abstract class DatabaseContext
{
    private DatabaseConnection _databaseConnection;
    private ModelBuilder _modelBuilder;
    private DatabaseMapper _databaseMapper;

    public DatabaseContext (string connectionString, IDbConnectionFactory factory)
    {

        _databaseConnection = new DatabaseConnection(connectionString, factory);
        _modelBuilder = new ModelBuilder();
        _databaseMapper = new DatabaseMapper(this);

    }

    public Table<T> Set<T>() where T : class
    {
        // Zwróć DbSet odpowiedni do typu T
        return new Table<T>(_databaseConnection);
    }

    protected abstract void OnModelCreating(ModelBuilder modelBuilder);

    private DataBaseMapping Map()
    {
        throw new NotImplementedException();
    }


}