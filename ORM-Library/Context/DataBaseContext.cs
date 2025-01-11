using ORMLibrary.Mapping;
using ORMLibrary.QueryBuilders;
using ORMLibrary.DataAccess;
using ORMLibrary.DataAccess.DatabaseConnectionFactories;
using ORMLibrary.Logging;
using System.Data;
using System.Text;

namespace ORMLibrary.Context{


public abstract class DatabaseContext
{
    private DatabaseConnection _databaseConnection;
    private DatabaseMapper _databaseMapper;

    private DatabaseMapping _databaseMapping;

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

        string dropForeignKeysQuery = 
            @"
            DO $$
            DECLARE
                r RECORD;
            BEGIN
                -- Iteruj przez wszystkie klucze obce w schemacie public
                FOR r IN 
                    SELECT 
                        tc.table_name, 
                        kcu.constraint_name
                    FROM 
                        information_schema.table_constraints AS tc
                        JOIN information_schema.key_column_usage AS kcu 
                        ON tc.constraint_name = kcu.constraint_name
                    WHERE 
                        tc.constraint_type = 'FOREIGN KEY' 
                        AND tc.table_schema = 'public'
                LOOP
                    -- Usuń każdy klucz obcy
                    EXECUTE FORMAT(
                        'ALTER TABLE %I DROP CONSTRAINT %I',
                        r.table_name, r.constraint_name
                    );
                END LOOP;
            END $$;
            ";

        if(_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(dropForeignKeysQuery) == 0)
        {
            logger.LogError("Bład przy usuwaniu kluczy", new Exception("Error while dripping foreign keys"));
        }
        logger.LogInfo("Foreign Keys Dropped");

        

        // Automatyczne mapowanie bazy danych
        DatabaseMapping databaseMapping = _databaseMapper.AutoMap();
        // Pobranie listy istniejących tabel w bazie danych
         List<string> existingTables = _databaseConnection
            .CreateQueryExecutor()
            .ExecuteQuery("SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'")
            .AsEnumerable()
            .Select(row => row["table_name"].ToString())
            .Where(tableName => tableName != null)
            .ToList()!;

        // Znalezienie tabel, które są w bazie danych, ale nie w mapowaniu
        List<string> tablesToRemove = existingTables
            .Except(databaseMapping.Tables.Select(t => t.TableName), StringComparer.OrdinalIgnoreCase)
            .ToList();

        // Usunięcie niepotrzebnych tabel
        foreach (string tableToRemove in tablesToRemove)
        {
            string dropQuery = new DropQuerryBuilder()
                .BuildDropTable(tableToRemove)
                .GetQuery();

            logger.LogInfo($"{dropQuery}");
            if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(dropQuery) == 0)
            {
                return false;
            }
        }

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

        // Usunięcie niepotrzebnych kolumn
        foreach (TableMapping table in databaseMapping.Tables)
        {
            var existingTable = existingTables.Find(t => t.Equals(table.TableName, StringComparison.OrdinalIgnoreCase));
            if (existingTable != null)
            {
                var existingColumns = _databaseConnection
                    .CreateQueryExecutor()
                    .ExecuteQuery($"SELECT column_name FROM information_schema.columns WHERE table_name = '{table.TableName}' AND table_schema = 'public'")
                    .AsEnumerable()
                    .Select(row => row["column_name"].ToString())
                    .Where(columnName => columnName != null)
                    .ToList()!;

                var columnsToRemove = existingColumns
                    .Except(table.Properties.Select(p => p.PropertyName), StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var columnsToAdd = table.Properties
                    .Select(p => p.PropertyName)
                    .Except(existingColumns, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach(string columnToAdd in columnsToAdd)
                {
                    if (!string.IsNullOrEmpty(columnToAdd))
                    {
                        string addColumnQuery = new AlterQuerryBuilder()
                            .BuildAlterTable(table.TableName)
                            .BuildAddColumn(columnToAdd, table.Properties.Find(p => p.PropertyName == columnToAdd).PropertyType.ToString())
                            .GetQuery();

                        logger.LogInfo($"Adding column: {columnToAdd} to table: {table.TableName}");
                        logger.LogInfo(addColumnQuery);
                        if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(addColumnQuery) == 0)
                        {
                            return false;
                        }
                    }
                }

                foreach (string columnToRemove in columnsToRemove)
                {
                    if (!string.IsNullOrEmpty(columnToRemove))
                    {
                        string dropColumnQuery = new AlterQuerryBuilder()
                            .BuildAlterTable(table.TableName)
                            .BuildDropColumn(columnToRemove)
                            .GetQuery();

                        logger.LogInfo($"Dropping column: {columnToRemove} from table: {table.TableName}");
                        logger.LogInfo(dropColumnQuery);
                        if (_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(dropColumnQuery) == 0)
                        {
                            return false;
                        }
                    }
                }
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

}
}

