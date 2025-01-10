using System;
using System.Data;
using System.Data.Common;
using Mapping;
using QueryBuilders;
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

    private bool Map()
    {
        DatabaseMapping databaseMapping = _databaseMapper.AutoMap();

        foreach(TableMapping table in databaseMapping.Tables)
        {
            
            var sqlBuilder = new CreateQuerryBuilder().BuildCreate(table.Name);

            foreach(PropertyMapping property in table.Properties)
            {
                sqlBuilder.BuildAddType(property.PropertyName, property.PropertyType, table.PrimaryKeys.Contains(property.PropertyName));
            }

            string query = sqlBuilder.BuildFinal().GetQuery();
            if(_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
            {
                return false;
            };
            
        }

        foreach(TableMapping table in databaseMapping.Tables)
        {

            foreach(ForeignKey foreignKey in table.ForeignKeys)
            {
                var constraintName = $"{table.TableName}_{foreignKey.ReferenceTable}_FK";
                var referenceTable = databaseMapping.Tables.Find(table => (table.TableName == foreignKey.ReferenceTable));
                
                if (referenceTable == null)
                {
                    throw new Exception("Reference Table doesn't exist!");
                    return false;
                }
                
                var referenceName = ""
                foreach (var primaryKey in referenceTable.PrimaryKeys)
                {
                    referenceName += $"{primaryKey}, "
                }
                referenceName.TrimEnd(",", " ");
                
                string query = new AlterQuerryBuilder()
                    .BuildAlterTable(table.TableName)
                    .BuildAddConstraint(constraintName)
                    .BuildForeignKey(foreignKey.Name)
                    .BuildReferences(foreignKey.ReferenceTable, referenceName)
                    .GetQuery();
                    
                if(_databaseConnection.CreateQueryExecutor().ExecuteNonQuery(query) == 0)
                {
                    return false;
                };
            }
        }
        return true;
    }


}