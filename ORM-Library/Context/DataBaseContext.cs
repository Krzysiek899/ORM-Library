using ORMLibrary.Mapping;
using ORMLibrary.QueryBuilders;
using ORMLibrary.DataAccess;
using ORMLibrary.DataAccess.DatabaseConnectionFactories;
using ORMLibrary.Logging;

namespace ORMLibrary.Context{


public abstract class DatabaseContext
{
    private DatabaseConnection _databaseConnection;
    private DatabaseMapper _databaseMapper;

    private DatabaseMapping? _databaseMapping;

    private Logger logger = Logger.GetInstance();

    public DatabaseContext (string connectionString, IDbConnectionFactory factory)
    {

        _databaseConnection = new DatabaseConnection(connectionString, factory);
        _databaseMapper = new DatabaseMapper(this);
        this.Map();
    }

    public DatabaseConnection GetDatabaseConnection()
    {
        return _databaseConnection;
    }

    public Table<T> Set<T>() where T : class
    {
        // Zwróć DbSet odpowiedni do typu T
        return new Table<T>(_databaseConnection);
    }

    private bool Map()
    {
        // Automatyczne mapowanie bazy danych
        DatabaseMapping databaseMapping = _databaseMapper.AutoMap();

        // Tworzenie tabel
        foreach (TableMapping table in databaseMapping.Tables)
        {
            var sqlBuilder = new CreateQuerryBuilder().BuildCreate(table.TableName);

            foreach (PropertyMapping property in table.Properties)
            {
                if (property.PropertyType.ToString() == "VARCHAR" && property.AdditionalModificators.ContainsKey(AdditionalModificator.VARCHAR_LEN))
                {
                    // Upewnij się, że VARCHAR uwzględnia MaxLength
                    sqlBuilder.BuildAddType(
                        property.PropertyName, 
                        property.PropertyType.ToString(), 
                        table.PrimaryKeys.Contains(property.PropertyName),
                        property.AdditionalModificators[AdditionalModificator.VARCHAR_LEN]
                    );
                }
                else
                {
                    sqlBuilder.BuildAddType(
                        property.PropertyName, 
                        property.PropertyType.ToString(), 
                        table.PrimaryKeys.Contains(property.PropertyName)
                    );
                }
            }

            // Budowanie zapytania CREATE
            string query = sqlBuilder.BuildFinal().GetQuery();
            logger.LogInfo(query);

            // Wykonanie zapytania
            if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
            {
                return false;
            }
        }

        // Tworzenie kluczy obcych
        foreach (TableMapping table in databaseMapping.Tables)
        {
            foreach (ForeignKey foreignKey in table.ForeignKeys)
            {
                var constraintName = $"{table.TableName}_{foreignKey.ReferenceTable}_FK";
                var referenceTable = databaseMapping.Tables.Find(refTable => (refTable.TableName == foreignKey.ReferenceTable));

                if (referenceTable == null)
                {
                    throw new Exception("Reference Table doesn't exist!");
                }

                // Pobranie kluczy głównych z tabeli referencyjnej
                var referenceName = "";
                foreach (var primaryKey in referenceTable.PrimaryKeys)
                {
                    referenceName += $"{primaryKey}, ";
                }
                referenceName = referenceName.TrimEnd(',', ' ');

                // Budowanie zapytania ALTER TABLE
                string query = new AlterQuerryBuilder()
                    .BuildAlterTable(table.TableName)
                    .BuildAddConstraint(constraintName)
                    .BuildForeignKey(foreignKey.Name)
                    .BuildReferences(foreignKey.ReferenceTable, referenceName)
                    .BuildFinal()
                    .GetQuery();

                logger.LogInfo($"New Query Alter: {query}");

                // Wykonanie zapytania
                if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool Map_test()
    {
        logger.LogInfo("Starting mapping process.");

        if (_databaseMapping == null)
        {
            logger.LogInfo("Initial database mapping is null, performing auto-mapping.");
            _databaseMapping = _databaseMapper.AutoMap();
        }

        DatabaseMapping newMapping = _databaseMapper.AutoMap();     // tutaj jest newMapping
        logger.LogInfo("New database mapping created.");

        // Sprawdzenie, czy są usunięte tabele
        foreach (TableMapping existingTable in _databaseMapping.Tables)
        {
            var newTable = newMapping.Tables.Find(t => t.TableName == existingTable.TableName);
            logger.LogInfo($"Checking table: {existingTable.TableName}");

            if (newTable == null)
            {
                // Tabela została usunięta - usuń ją z bazy danych
                string query = new DropQuerryBuilder()
                    .BuildDropTable(existingTable.TableName)
                    .GetQuery();

                logger.LogInfo($"Drop table query: {query}");
                if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                {
                    logger.LogError($"Error dropping table: {existingTable.TableName}", new Exception());
                    return false; // Błąd w usuwaniu tabeli
                }
            }
        }

        // Sprawdzenie, czy są usunięte kolumny
        foreach (TableMapping newTable in newMapping.Tables)
        {
            var existingTable = _databaseMapping.Tables.Find(t => t.TableName == newTable.TableName);
            if (existingTable != null)
            {
                logger.LogInfo($"Checking columns in existing table: {existingTable.TableName}");

                // Istniejąca tabela - sprawdź brakujące kolumny
                foreach (PropertyMapping existingProperty in existingTable.Properties)
                {
                    var newProperty = newTable.Properties.Find(p => p.PropertyName == existingProperty.PropertyName);

                    if (newProperty == null)
                    {
                        // Kolumna została usunięta - usuń ją z tabeli
                        string query = new AlterQuerryBuilder()
                            .BuildAlterTable(existingTable.TableName)
                            .BuildDropColumn(existingProperty.PropertyName)
                            .BuildFinal()
                            .GetQuery();

                        logger.LogInfo($"Drop column query: {query}");
                        if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                        {
                            logger.LogError($"Error dropping column: {existingProperty.PropertyName} from table: {existingTable.TableName}", new Exception());
                            return false; // Błąd w usuwaniu kolumny
                        }
                    }
                }
            }
        }

        // Porównanie tabel
        foreach (TableMapping newTable in newMapping.Tables)
        {
            var existingTable = _databaseMapping.Tables.Find(t => t.TableName == newTable.TableName);

            logger.LogInfo("Finding: " + newTable.TableName + "   Tables in _databaseMapping.Tables");
            foreach(TableMapping table in _databaseMapping.Tables)
            {
                logger.LogInfo(table.TableName);
            }

            logger.LogInfo($"Checking table: {newTable.TableName}");

            if (existingTable == null)
            {
                // Nowa tabela - utwórz ją
                var sqlBuilder = new CreateQuerryBuilder().BuildCreate(newTable.TableName);

                foreach (PropertyMapping property in newTable.Properties)
                {
                    if (property.AdditionalModificators.ContainsKey(AdditionalModificator.VARCHAR_LEN))
                    {
                        sqlBuilder.BuildAddType(property.PropertyName, TypeTranslator.Translate(property.PropertyType.ToString()).ToString(), newTable.PrimaryKeys.Contains(property.PropertyName), property.AdditionalModificators[AdditionalModificator.VARCHAR_LEN]);
                    }
                    else
                    {
                        sqlBuilder.BuildAddType(property.PropertyName, TypeTranslator.Translate(property.PropertyType.ToString()).ToString(), newTable.PrimaryKeys.Contains(property.PropertyName));
                    }
                }

                string query = sqlBuilder.BuildFinal().GetQuery();
                logger.LogInfo($"Create table query:\n {query}");

                if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                {
                    logger.LogError($"Error creating table: {newTable.TableName}", new Exception());
                    return false;
                }
            }
            else
            {
                // Istniejąca tabela - sprawdź zmiany w kolumnach
                foreach (PropertyMapping newProperty in newTable.Properties)
                {
                    var existingProperty = existingTable.Properties.Find(p => p.PropertyName == newProperty.PropertyName);

                    if (existingProperty == null)
                    {
                        // Nowa kolumna - dodaj ją
                        string query = new AlterQuerryBuilder()
                            .BuildAlterTable(newTable.TableName)
                            .BuildAddColumn(newProperty.PropertyName, TypeTranslator.Translate(newProperty.PropertyType.ToString()).ToString())
                            .BuildFinal()
                            .GetQuery();

                        logger.LogInfo($"Add column query: {query}");

                        if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                        {
                            logger.LogError($"Error adding column: {newProperty.PropertyName} to table: {newTable.TableName}", new Exception());
                            return false;
                        }
                    }
                    else if (existingProperty.PropertyType != newProperty.PropertyType)
                    {
                        // Typ danych kolumny zmienił się - dostosuj
                        string query = new AlterQuerryBuilder()
                            .BuildAlterTable(newTable.TableName)
                            .BuildModifyColumn(newProperty.PropertyName, TypeTranslator.Translate(newProperty.PropertyType.ToString()).ToString())
                            .BuildFinal()
                            .GetQuery();

                        logger.LogInfo($"Modify column query: {query}");

                        if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                        {
                            logger.LogError($"Error modifying column: {newProperty.PropertyName} in table: {newTable.TableName}", new Exception());
                            return false;
                        }
                    }
                }
            }
        }

        // Porównanie kluczy obcych
        foreach (TableMapping newTable in newMapping.Tables)
        {
            foreach (ForeignKey foreignKey in newTable.ForeignKeys)
            {
                var existingTable = _databaseMapping.Tables.Find(t => t.TableName == newTable.TableName);

                if (existingTable != null && !existingTable.ForeignKeys.Exists(fk => fk.Name == foreignKey.Name))
                {
                    // Nowy klucz obcy - dodaj go
                    var constraintName = $"{newTable.TableName}_{foreignKey.ReferenceTable}_FK";
                    var referenceTable = newMapping.Tables.Find(refTable => refTable.TableName == foreignKey.ReferenceTable);

                    if (referenceTable == null)
                    {
                        logger.LogError("Reference Table doesn't exist!",new Exception());
                        throw new Exception("Reference Table doesn't exist!");
                    }

                    var referenceName = string.Join(", ", referenceTable.PrimaryKeys);

                    string query = new AlterQuerryBuilder()
                        .BuildAlterTable(newTable.TableName)
                        .BuildAddConstraint(constraintName)
                        .BuildForeignKey(foreignKey.Name)
                        .BuildReferences(foreignKey.ReferenceTable, referenceName)
                        .BuildFinal()
                        .GetQuery();

                    logger.LogInfo($"Add foreign key query: {query}");
                    if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                    {
                        logger.LogError($"Error adding foreign key: {foreignKey.Name} to table: {newTable.TableName}", new Exception());
                        return false;
                    }
                }
            }
        }

        // Aktualizuj stan mappingu
        _databaseMapping = newMapping;
        logger.LogInfo("Mapping process completed successfully.");
        return true;
    }    

//     public bool Update()
//     {
//         if (_databaseMapping == null)
//         {
//             return false;
//         }

//         DatabaseMapping newMapping = _databaseMapper.AutoMap();

//         foreach (TableMapping newTable in newMapping.Tables)
//         {
//             var existingTable = _databaseMapping.Tables.Find(t => t.TableName == newTable.TableName);

//             if (existingTable == null)
//             {
//                 var sqlBuilder = new CreateQueryBuilder().BuildCreate(newTable.TableName);

//                 foreach (PropertyMapping property in newTable.Properties)
//                 {
//                     sqlBuilder.BuildAddType(property.PropertyName, TypeTranslator.Translate(property.PropertyType.ToString()), newTable.PrimaryKeys.Contains(property.PropertyName));
//                 }

//                 string query = sqlBuilder.BuildFinal().GetQuery();
//                 logger.LogInfo(query);

//                 if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
//                 {
//                     return false;
//                 }
//             }
//             else
//             {
//                 foreach (PropertyMapping newProperty in newTable.Properties)
//                 {
//                     var existingProperty = existingTable.Properties.Find(p => p.PropertyName == newProperty.PropertyName);

//                     if (existingProperty == null)
//                     {
//                         string query = new AlterQueryBuilder()
//                             .BuildAlterTable(newTable.TableName)
//                             .BuildAddColumn(newProperty.PropertyName, TypeTranslator.Translate(newProperty.PropertyType.ToString()))
//                             .BuildFinal()
//                             .GetQuery();

//                         logger.LogInfo(query);

//                         if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
//                         {
//                             return false;
//                         }
//                     }
//                 }
//             }
//         }
//         foreach (TableMapping newTable in newMapping.Tables)
//         {
//             foreach (ForeignKey foreignKey in newTable.ForeignKeys)
//             {
//                 var constraintName = $"{newTable.TableName}_{foreignKey.ReferenceTable}_FK";
//                 var referenceTable = newMapping.Tables.Find(refTable => (refTable.TableName == foreignKey.ReferenceTable));

//                 if (referenceTable == null)
//                 {
//                     throw new Exception("Reference Table doesn't exist!");
//                 }

//                 var referenceName = "";
//                 foreach (var primaryKey in referenceTable.PrimaryKeys)
//                 {
//                     referenceName += $"{primaryKey}, ";
//                 }
//                 referenceName = referenceName.TrimEnd(',', ' ');

//                 string query = new AlterQueryBuilder()
//                     .BuildAlterTable(newTable.TableName)
//                     .BuildAddConstraint(constraintName)
//                     .BuildForeignKey(foreignKey.Name)
//                     .BuildReferences(foreignKey.ReferenceTable, referenceName)
//                     .BuildFinal()
//                     .GetQuery();

//                 logger.LogInfo($"New Query Alter: {query}");
//                 if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
//                 {
//                     return false;
//                 }
//             }
//         }

//         _databaseMapping = newMapping;
//         return true;
//     }



  }
 

}