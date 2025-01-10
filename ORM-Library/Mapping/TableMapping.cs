namespace Mapping
{
    public class TableMapping{
        public string TableName { get; set; }
        public List<string> PrimaryKeys {get;} = new ();

        public List<ForeignKey> ForeignKeys {get;} = new ();

        public List<PropertyMapping> Properties {get;}= new();

        public TableMapping(string tableName)
        {
            TableName = tableName;
        }
        
        public void AddProperty(PropertyMapping property){
            Properties.Add(property);
        }

        public void AddPrimaryKey(string primaryKey) {
            PrimaryKeys.Add(primaryKey);
        }

        public void AddForeignKey(ForeignKey foreignKey){
            ForeignKeys.Add(foreignKey);
        }
        
    }

}