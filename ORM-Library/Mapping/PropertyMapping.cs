namespace ORMLibrary.Mapping {

    public class PropertyMapping
    {
        public PropertyType PropertyType {get; set;}

        public Dictionary<AdditionalModificator, string> AdditionalModificators {get;} = new Dictionary<AdditionalModificator, string>();

        public string PropertyName {get; set;}

        public PropertyMapping(PropertyType propertyType, string propertyName){
            PropertyType = propertyType;
            PropertyName = propertyName;
        }

        public void addModificator(AdditionalModificator modificator, string modificatorValue){
            AdditionalModificators.Add(modificator, modificatorValue);
        }

    }

    public enum AdditionalModificator{
        VARCHAR_LEN
    }
}