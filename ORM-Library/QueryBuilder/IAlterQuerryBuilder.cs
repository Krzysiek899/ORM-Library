namespace ORMLibrary.QueryBuilders
{
    public interface IAlterBuilder
    {
        IAlterBuilder BuildAlterTable(string tables);
        IAlterBuilder BuildAddColumn(string column, string type, string maxLength = "256");
        IAlterBuilder BuildDropColumn(string column);
        IAlterBuilder BuildAlterColumn(string column, string type, string dbType, string maxLength = "256");
        IAlterBuilder BuildChangeColumn(string column, string newColumn, string type);
        IAlterBuilder BuildRenameTable(string table);
        IAlterBuilder BuildRenameColumn(string column, string newColumn);
        IAlterBuilder BuildAddPrimaryKey(string column);
        IAlterBuilder BuildAddConstraint(string key);
        IAlterBuilder BuildForeignKey(string key);
        IAlterBuilder BuildReferences(string targetTable, string targetColumn);

        IAlterBuilder BuildDropConstraint(string key);
        IAlterBuilder BuildDropForeignKey(string keyName);

        IAlterBuilder BuildFinal();

        string GetQuery();
    }
}