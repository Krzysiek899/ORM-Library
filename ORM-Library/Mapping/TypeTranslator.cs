namespace ORMLibrary.Mapping
{
    public class TypeTranslator
    {
        public static PropertyType Translate(string typeToTraslate, int? numberOfChars = 255, string translationType = "modelToDb")
        {
            if(translationType == "modelToDb") //z modelu do bazy
            {
                switch(typeToTraslate)
                {
                    case "Int32":
                        return PropertyType.INT;
                    case "String":
                        return PropertyType.VARCHAR;
                    case "Decimal":
                        return PropertyType.DECIMAL;
                    case "DateTime":
                        return PropertyType.TIMESTAMP;
                    case "Double":
                        return PropertyType.DOUBLE;
                    case "Boolean":
                        return PropertyType.BOOLEAN;
                    default:
                        throw new ArgumentException($"Unsupported type {typeToTraslate}", nameof(typeToTraslate));
                }
            }
            else if(translationType == "PostgreSQLToModel") // z bazy PostgreSQL do modelu
            {
                switch(typeToTraslate)
                {
                    case "integer":
                        return PropertyType.INT;
                    case "character varying":  
                        return PropertyType.VARCHAR; 
                    case "timestamp without time zone":
                        return PropertyType.DATETIME; 
                    case "numeric":
                        return PropertyType.DECIMAL;
                    case "boolean":
                        return PropertyType.BOOLEAN;
                    case "double precision":
                        return PropertyType.DOUBLE;
                    default:
                        throw new ArgumentException($"Unexpected type {typeToTraslate}", nameof(typeToTraslate));
                }   
            }
            else if(translationType == "MySQLToModel") // z bazy MySQL do modelu
            {
                switch(typeToTraslate)
                {
                    case "int":
                        return PropertyType.INT;
                    case "varchar":  
                        return PropertyType.VARCHAR; 
                    case "timestamp":
                        return PropertyType.DATETIME; 
                    case "decimal":
                        return PropertyType.DECIMAL;
                    case "tinyint":
                        return PropertyType.BOOLEAN; 
                    case "double":
                        return PropertyType.DOUBLE;
                    default:
                        throw new ArgumentException($"Unexpected type {typeToTraslate}", nameof(typeToTraslate));
                }
            }
            else 
            {
                throw new ArgumentException($"Unexpected translation type {translationType}");
            }
        }
    }
}