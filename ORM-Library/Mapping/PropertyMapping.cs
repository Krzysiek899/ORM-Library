public class PropertyMapping
{
    public PropertyType PropertyType {get; set;}
    public string PropertyName {get; set;}

    public PropertyMapping(PropertyType propertyType, string propertyName){
        PropertyType = propertyType;
        PropertyName = propertyName;
    }

}