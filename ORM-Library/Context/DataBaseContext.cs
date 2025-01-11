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
        if (_databaseMapping == null)
        {
            _databaseMapping = _databaseMapper.AutoMap();
        }

        DatabaseMapping newMapping = _databaseMapper.AutoMap();

        // Porównanie tabel
        foreach (TableMapping newTable in newMapping.Tables)
        {
            var existingTable = _databaseMapping.Tables.Find(t => t.TableName == newTable.TableName);

            if (existingTable == null)
            {
                // Nowa tabela - utwórz ją
                var sqlBuilder = new CreateQueryBuilder().BuildCreate(newTable.TableName);

                foreach (PropertyMapping property in newTable.Properties)
                {
                    sqlBuilder.BuildAddType(property.PropertyName, TypeTranslator.Translate(property.PropertyType.ToString()), newTable.PrimaryKeys.Contains(property.PropertyName));
                }

                string query = sqlBuilder.BuildFinal().GetQuery();
                logger.LogInfo(query);

                if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                {
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
                        string query = new AlterQueryBuilder()
                            .BuildAlterTable(newTable.TableName)
                            .BuildAddColumn(newProperty.PropertyName, TypeTranslator.Translate(newProperty.PropertyType.ToString()))
                            .BuildFinal()
                            .GetQuery();

                        logger.LogInfo(query);

                        if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                        {
                            return false;
                        }
                    }
                    else if (existingProperty.PropertyType != newProperty.PropertyType)
                    {
                        // Typ danych kolumny zmienił się - dostosuj
                        string query = new AlterQueryBuilder()
                            .BuildAlterTable(newTable.TableName)
                            .BuildModifyColumn(newProperty.PropertyName, TypeTranslator.Translate(newProperty.PropertyType.ToString()))
                            .BuildFinal()
                            .GetQuery();

                        logger.LogInfo(query);

                        if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                        {
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
                        throw new Exception("Reference Table doesn't exist!");
                    }

                    var referenceName = string.Join(", ", referenceTable.PrimaryKeys);

                    string query = new AlterQueryBuilder()
                        .BuildAlterTable(newTable.TableName)
                        .BuildAddConstraint(constraintName)
                        .BuildForeignKey(foreignKey.Name)
                        .BuildReferences(foreignKey.ReferenceTable, referenceName)
                        .BuildFinal()
                        .GetQuery();

                    logger.LogInfo($"New Query Alter: {query}");
                    if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                    {
                        return false;
                    }
                }
            }
        }

        // Aktualizuj stan mappingu
        _databaseMapping = newMapping;
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
 }