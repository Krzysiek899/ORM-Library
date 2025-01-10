namespace QueryBuilders
{
    public interface IAlterBuilder
    {
        IAlterBuilder BuildAlterTable(string tables);
        IAlterBuilder BuildAddColumn(string column, string type);
        IAlterBuilder BuildDropColumn(string column);
        IAlterBuilder BuildModifyColumn(string column, string type);
        IAlterBuilder BuildChangeColumn(string column, string newColumn, string type);
        IAlterBuilder BuildRenameTable(string table);
        IAlterBuilder BuildRenameColumn(string column, string newColumn);
        IAlterBuilder BuildAddPrimaryKey(string column);

    }
}