using ORMLibrary.Mapping;
using ORMLibrary.QueryBuilders;
using ORMLibrary.DataAccess;
using ORMLibrary.DataAccess.DatabaseConnectionFactories;
using ORMLibrary.Logging;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Npgsql;

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

    private string DetectDatabaseType()
    {
        var connection = _databaseConnection.GetConnection();
        
        if (connection is MySqlConnection)
        {
            return "MySQL";
        }
        else if (connection is NpgsqlConnection)
        {
            return "PostgreSQL";
        }
        
        throw new InvalidOperationException("Unsupported database type");
    }


    private bool Map()
    {

        string databaseType = DetectDatabaseType();

        string foreignKeysQuery = databaseType == "MySQL" ? 
        @"
            SELECT 
                table_name, 
                constraint_name 
            FROM 
                information_schema.KEY_COLUMN_USAGE
            WHERE 
                REFERENCED_TABLE_NAME IS NOT NULL 
                AND TABLE_SCHEMA = DATABASE();
        "
        :
        @"
            SELECT 
                kcu.table_name, 
                tc.constraint_name
            FROM 
                information_schema.table_constraints AS tc
            JOIN 
                information_schema.key_column_usage AS kcu 
                ON tc.constraint_name = kcu.constraint_name
            WHERE 
                tc.constraint_type = 'FOREIGN KEY'
                AND tc.table_schema = 'public';
        ";

        
        var foreignKeys = _databaseConnection
            .CreateQueryExecutor()
            .ExecuteQuery(foreignKeysQuery)
            .AsEnumerable()
            .Select(row => (
                Name : row["constraint_name"].ToString(),
                Table : row["table_name"].ToString()
            ))
            .ToList()!;
        
        foreach (var foreignKey in foreignKeys)
        {
            logger.LogInfo($"Foreign Key: {foreignKey.Name} {foreignKey.Table}");
            var alterQuerryBuilder = new AlterQuerryBuilder();
            string dropQuery = "";
            if(databaseType == "MySQL")
            {
                if(foreignKey.Table != null)
                {
                    dropQuery = alterQuerryBuilder.BuildAlterTable(foreignKey.Table).BuildDropForeignKey(foreignKey.Name).BuildFinal().GetQuery();
                }

            }
            else
            {
                if(foreignKey.Table != null)
                {
                    dropQuery = alterQuerryBuilder.BuildAlterTable(foreignKey.Table).BuildDropConstraint(foreignKey.Name).BuildFinal().GetQuery();
                }
            }
            _databaseConnection.CreateQueryExecutor().ExecuteNonQuery(dropQuery);
        }


        // string dropForeignKeysQuery = databaseType == "MySQL"
        // ? @"DELIMITER $$
// 
        // CREATE PROCEDURE DropAllForeignKeys()
        // BEGIN
        //     DECLARE done INT DEFAULT 0;
        //     DECLARE table_name VARCHAR(255);
        //     DECLARE constraint_name VARCHAR(255);
        //     DECLARE cur CURSOR FOR 
        //         SELECT table_name, constraint_name
        //         FROM information_schema.key_column_usage 
        //         WHERE table_schema = DATABASE();
            
        //     DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = 1;
            
        //     OPEN cur;
            
        //     read_loop: LOOP
        //         FETCH cur INTO table_name, constraint_name;
        //         IF done THEN
        //             LEAVE read_loop;
        //         END IF;
                
        //         SET @sql = CONCAT('ALTER TABLE ', table_name, ' DROP FOREIGN KEY ', constraint_name);
        //         PREPARE stmt FROM @sql;
        //         EXECUTE stmt;
        //         DEALLOCATE PREPARE stmt;
        //     END LOOP;
            
        //     CLOSE cur;
        // END$$

        // DELIMITER ;
        // "
        // : @"
        //     DO $$
        //     DECLARE
        //         r RECORD;
        //     BEGIN
        //         -- Iteruj przez wszystkie klucze obce w schemacie public
        //         FOR r IN 
        //             SELECT 
        //                 tc.table_name, 
        //                 kcu.constraint_name
        //             FROM 
        //                 information_schema.table_constraints AS tc
        //                 JOIN information_schema.key_column_usage AS kcu 
        //                 ON tc.constraint_name = kcu.constraint_name
        //             WHERE 
        //                 tc.constraint_type = 'FOREIGN KEY' 
        //                 AND tc.table_schema = 'public'
        //         LOOP
        //             -- Usuń każdy klucz obcy
        //             EXECUTE FORMAT(
        //                 'ALTER TABLE %I DROP CONSTRAINT %I',
        //                 r.table_name, r.constraint_name
        //             );
        //         END LOOP;
        //     END $$;
        //     ";
        

        //_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(dropForeignKeysQuery);
        logger.LogInfo("Foreign Keys Dropped");

        

        // Automatyczne mapowanie bazy danych
        DatabaseMapping databaseMapping = _databaseMapper.AutoMap();
        // Pobranie listy istniejących tabel w bazie danych

        string getTablesQuery = databaseType == "MySQL"
        ? "SELECT table_name FROM information_schema.tables WHERE table_schema = DATABASE();"
        : "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';";


         List<string> existingTables = _databaseConnection
            .CreateQueryExecutor()
            .ExecuteQuery(getTablesQuery)
            .AsEnumerable()
            .Select(row => row["table_name"].ToString())
            .Where(tableName => tableName != null)
            .ToList()!;

        logger.LogInfo($"Existing tables: {string.Join(", ", existingTables)}");

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
            _databaseConnection.CreateQueryExecutor().ExecuteNonQuery(dropQuery);
        }

        // Tworzenie tabel
        logger.LogInfo($"Tables in mapping: {string.Join(", ", databaseMapping.Tables.Select(t => t.TableName))}");

        foreach (TableMapping table in databaseMapping.Tables)
        {
            logger.LogInfo($"Current processing table: {table}");
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
            _databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query);
        }

        // Usunięcie niepotrzebnych kolumn
        foreach (TableMapping table in databaseMapping.Tables)
        {
            var existingTable = existingTables.Find(t => t.Equals(table.TableName, StringComparison.OrdinalIgnoreCase));
            if (existingTable != null)
            {
                 string getColumnsQuery = databaseType == "MySQL"
                ? $"SELECT column_name, data_type FROM information_schema.columns WHERE table_name = '{table.TableName}' AND table_schema = DATABASE();"
                : $"SELECT column_name, data_type FROM information_schema.columns WHERE table_name = '{table.TableName}' AND table_schema = 'public';";

                var existingColumns = _databaseConnection
                    .CreateQueryExecutor()
                    .ExecuteQuery(getColumnsQuery)
                    .AsEnumerable()
                    .Select(row => (
                        Name : row["column_name"].ToString(),
                        Type : row["data_type"].ToString()
                    ))
                    .Where(column => column.Name != null && column.Type != null)
                    .ToList()!;

                var columnsToRemove = existingColumns
                    .Select(c => c.Name)
                    .Except(table.Properties.Select(p => p.PropertyName), StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var columnsToAdd = table.Properties
                    .Select(p => p.PropertyName)
                    .Except(existingColumns.Select(c => c.Name), StringComparer.OrdinalIgnoreCase)
                    .ToList();
    

                var columnsToEdit = table.Properties
                    .Where(p => existingColumns.Any(ec => 
                        ec.Name.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase) &&
                        !TypeTranslator.Translate(ec.Type, translationType: databaseType == "MySQL" ? "MySQLToModel" : "PostgreSQLToModel").ToString().Equals(p.PropertyType.ToString(), StringComparison.OrdinalIgnoreCase) 
                    ))
                    .Select(p => p.PropertyName)
                    .ToList();


                foreach(string columnToEdit in columnsToEdit)
                {
                   if(!string.IsNullOrEmpty(columnToEdit))
                   {
                        var property = table.Properties.Find(p => p.PropertyName == columnToEdit);
                        var alterQuerryBuilder = new AlterQuerryBuilder();
                        string editColumnQuery;
                        if(property.PropertyType.ToString() == "VARCHAR" && property.AdditionalModificators.ContainsKey(AdditionalModificator.VARCHAR_LEN))
                        {
                            editColumnQuery = alterQuerryBuilder
                                .BuildAlterTable(table.TableName)
                                .BuildAlterColumn(columnToEdit, property.PropertyType.ToString(), databaseType,  property.AdditionalModificators[AdditionalModificator.VARCHAR_LEN])
                                .GetQuery();
                        }
                        else
                        {
                            editColumnQuery = alterQuerryBuilder
                            .BuildAlterTable(table.TableName)
                            .BuildAlterColumn(columnToEdit, property.PropertyType.ToString(), databaseType)
                            .GetQuery();
                        }
      
                        logger.LogInfo(editColumnQuery);
                        _databaseConnection.CreateQueryExecutor().ExecuteNonQuery(editColumnQuery);
                   } 
                }
                
                foreach(string columnToAdd in columnsToAdd)
                {
                    if (!string.IsNullOrEmpty(columnToAdd))
                    {
                        var property = table.Properties.Find(p => p.PropertyName == columnToAdd);
                        var alterQuerryBuilder = new AlterQuerryBuilder();
                        string addColumnQuery;
                        if(property.PropertyType.ToString() == "VARCHAR" && property.AdditionalModificators.ContainsKey(AdditionalModificator.VARCHAR_LEN))
                        {
                            addColumnQuery = new AlterQuerryBuilder()
                            .BuildAlterTable(table.TableName)
                            .BuildAddColumn(columnToAdd, table.Properties.Find(p => p.PropertyName == columnToAdd).PropertyType.ToString(), property.AdditionalModificators[AdditionalModificator.VARCHAR_LEN])
                            .GetQuery();
                        }
                        else
                        {
                            addColumnQuery = new AlterQuerryBuilder()
                            .BuildAlterTable(table.TableName)
                            .BuildAddColumn(columnToAdd, table.Properties.Find(p => p.PropertyName == columnToAdd).PropertyType.ToString())
                            .GetQuery();
                        }

                        logger.LogInfo(addColumnQuery);
                        _databaseConnection.CreateQueryExecutor().ExecuteNonQuery(addColumnQuery);
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

                        logger.LogInfo(dropColumnQuery);
                        _databaseConnection.CreateQueryExecutor().ExecuteNonQuery(dropColumnQuery);
                    }
                }
            }
        }

        //edycja zmienionych kolumna


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
                _databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query);
            }
        }

        return true;
    }

}
}

