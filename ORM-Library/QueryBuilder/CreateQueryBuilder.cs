namespace QueryBuilders{
    public class CreateQuerryBuilder : ICreateBuilder
    {
        private string _createQuerry = "";
        public ICreateBuilder BuildCreate(string tableName)
        {
            _createQuerry += "CREATE TABLE " + tableName + "(\n";
            return this;
        }

        public ICreateBuilder BuildAddType(string columnName, string type, bool isPrimaryKey)
        {
            _createQuerry += "\t" + columnName + " " + type + " ";
            if(isPrimaryKey)
            {
                _createQuerry += "PRIMARY KEY,\n";
            }    
            return this;
        }

        public ICreateBuilder BuildForeignKey(string foreignKey, string referenceTable, string referenceKey)
        {
            _createQuerry += "FOREIGN KEY (" + foreignKey + ") REFERENCES " + referenceTable + " (" + referenceKey + "),\n";
            return this;
        }

        public ICreateBuilder BuildFinal()
        {
            _createQuerry.TrimEnd(',', ' ');
            _createQuerry += ");";
            return this;
        }

        public string GetQuery()
        {
            return _createQuerry;
        }
    }
}