namespace ORMLibrary.QueryBuilders
{
    public interface IDropBuilder 
    {
        IDropBuilder BuildDropTable(string tableName);
        IDropBuilder BuildDropDatabase(string databaseName);
        public string GetQuery();
    }
}