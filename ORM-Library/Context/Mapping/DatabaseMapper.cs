public class DatabaseMapper{
    private readonly DatabaseConnection _databaseConnection;
    private readonly ModelBuilder _modelBuilder;
    private readonly CreateQuerryBuilder _createQuerryBuilder;

    public DatabaseMapper(DatabaseConnection databaseConnection, ModelBuilder modelBuilder)
    {
        _databaseConnection = databaseConnection;
        _modelBuilder = modelBuilder;
        _createQuerryBuilder = new CreateQuerryBuilder();
    }

    public bool MapToDatabase()
    {
        var entityConfigurations = _modelBuilder.GetConfigurations();

        foreach (var config in entityConfigurations)
        {
            string createTableQuery = _createQuerryBuilder.BuildCreate();
            ExecuteQuery(createTableQuery);

            MapRelations(config);
        }

        return true;
    }

    

}