namespace Mapping
{
    public class TableMapping{
        public string TableName { get; set; }
        private readonly List<string> primaryKeys = new ();

        private readonly List<ForeignKey> foreignKeys = new ();

        private readonly List<PropertyMapping> properties = new();

        public TableMapping(string tableName)
        {
            TableName = tableName;
        }
        
        public void AddProperty(PropertyMapping property){
            properties.Add(property);
        }

        public void AddPrimaryKey(string primaryKey) {
            primaryKeys.Add(primaryKey);
        }

        public void AddForeignKey(ForeignKey foreignKey){
            foreignKeys.Add(foreignKey);
        }
        
    }

}