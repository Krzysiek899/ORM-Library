namespace ORMLibrary.Mapping
{
    public class TypeTranslator
    {
        public static PropertyType Translate(string typeToTraslate, int? numberOfChars = 255)
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
                default:
                    throw new ArgumentException($"Unsupported type {typeToTraslate}", nameof(typeToTraslate));
            }
        }
    }
}