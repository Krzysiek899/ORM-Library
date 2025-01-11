namespace ORMLibrary.QueryBuilders{    public class CreateQuerryBuilder : ICreateBuilder
    {
        private string _createQuerry = "";
        public ICreateBuilder BuildCreate(string tableName)
        {
            _createQuerry += "CREATE TABLE IF NOT EXISTS " + tableName + "(\n";
            return this;
        }

        public ICreateBuilder BuildAddType(string columnName, string type, bool isPrimaryKey, string maxLength = "256")
        {   
            if(type == "VARCHAR")
            {
                type += "(" + maxLength + ")";
            }
            _createQuerry += "\t" + columnName + " " + type;
            if (isPrimaryKey)
            {
                _createQuerry += " PRIMARY KEY,\n";
            }
            else
            {
                _createQuerry += ",\n";
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
            if (_createQuerry.EndsWith(",\n"))
            {
                _createQuerry = _createQuerry.Substring(0, _createQuerry.Length - 2) + "\n";
            }
            _createQuerry += ");";
            return this;
        }

        public string GetQuery()
        {
            return _createQuerry;
        }
    }
}