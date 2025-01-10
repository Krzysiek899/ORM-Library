public interface IDropBuilder 
{
    IDropBuilder BuildDropTable(string tableName);
    IDropBuilder BuildDropDatabase(string databaseName);
}