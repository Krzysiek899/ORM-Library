using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using ORMLibrary.Logging;
using ORMLibrary.Context;
using Microsoft.VisualBasic;

namespace ORMLibrary.Mapping
{


    public class DatabaseMapper{

        private readonly DatabaseContext _databaseContext;

        private readonly Logger logger = Logger.GetInstance();

        public DatabaseMapper( DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        private bool IsSimpleType(Type type)
        {
            return type.IsPrimitive || 
                type.IsValueType || 
                type == typeof(string);
        }


        public DatabaseMapping AutoMap()
        {
            var databaseMapping = new DatabaseMapping();

            var dbContextType = _databaseContext.GetType();
            
            // Znajdź wszystkie właściwości, które są typu TableContext
            var tableProperties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Table<>))
                .ToList();

            

            foreach (var tableProperty in tableProperties)
            {
                // Pobranie typu T (klasy kontekstowej)
                var tableType = tableProperty.PropertyType.GetGenericArguments()[0];

                logger.LogInfo($"Found Table for type: {tableType.Name}");

                var attributes = tableType.GetCustomAttributes(true);

                var tableName = tableType.Name;
                
                // Iteruj przez wszystkie atrybuty
                foreach (var attribute in attributes)
                    {
                        // Sprawdź, czy atrybut jest typu Table
                        if (attribute is TableAttribute tableAttribute)
                        {
                            logger.LogInfo($"Found Table Attribute: {tableAttribute.Name}");
                            tableName = tableAttribute.Name;
                        }
                    }

                var tableMapping = new TableMapping(tableName);
                
                var properties = tableType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => IsSimpleType(p.PropertyType))
                .ToList();
                
                foreach (var property in properties)
                {
                    logger.LogInfo($"  - Property: {property.Name}");

                    PropertyType propertyType = TypeTranslator.Translate(property.PropertyType.Name);

                    var propertyMapping = new PropertyMapping(propertyType, property.Name);
                    
                    var maxLengthAttribute = property.GetCustomAttribute<MaxLengthAttribute>();
                    if( maxLengthAttribute != null)
                    {
                        propertyMapping.addModificator(AdditionalModificator.VARCHAR_LEN, maxLengthAttribute.Length.ToString());
                    }

                    tableMapping.AddProperty(propertyMapping);

                    if(property.GetCustomAttribute<KeyAttribute>() != null)
                    {
                        tableMapping.AddPrimaryKey(property.Name);
                    }

                    //atrybut ForeignKey
                    var foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKeyAttribute != null)
                    {
                        logger.LogInfo($"Property '{property.Name}' has a ForeignKey attribute pointing to '{foreignKeyAttribute.Name}'");

                        var foreignKey = new ForeignKey(property.Name, foreignKeyAttribute.Name);

                        tableMapping.AddForeignKey(foreignKey);
                    }
                }

                databaseMapping.AddTable(tableMapping);
            }

        return databaseMapping;
            
        }


    }

}