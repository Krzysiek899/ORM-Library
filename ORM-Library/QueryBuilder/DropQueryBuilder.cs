namespace ORMLibrary.QueryBuilders
{
    public class DropQuerryBuilder : IDropBuilder 
    {
        private string _dropQuerry = "";

        public IDropBuilder BuildDropTable(string tableName)
        {
            _dropQuerry += "DROP TABLE " + tableName + "\n";
            return this;
        }
        public IDropBuilder BuildDropDatabase(string databaseName)
        {
            _dropQuerry += "DROP DATABASE " + databaseName + "\n";
            return this;
        }

        public string GetQuery(){
            return _dropQuerry + ";";
        }

        
    }
}