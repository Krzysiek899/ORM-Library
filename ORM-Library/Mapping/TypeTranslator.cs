public class TypeTranslator
{
    public static PropertyType Translate(string typeToTraslate, int? numberOfChars = 255)
    {
        switch(typeToTraslate)
        {
            case "int":
                return PropertyType.INT;
            case "string":
                return PropertyType.VARCHAR;
            case "decimal":
                return PropertyType.DECIMAL;
            case "DateTime":
                return PropertyType.DATETIME;
            case "double":
                return PropertyType.DOUBLE;
            default:
                throw new ArgumentException($"Nieobslugiwany typ {typeToTraslate}", nameof(typeToTraslate));
        }
    }
}