namespace ORMLibrary.QueryBuilders
{
    public interface ICreateBuilder
    {
        public ICreateBuilder BuildCreate(string tableName);
        public ICreateBuilder BuildAddType(string columnName, string type, string? maxLength ,bool isPrimaryKey);
        public ICreateBuilder BuildForeignKey(string foreignKey, string referenceTable, string referenceKey);
        public ICreateBuilder BuildFinal();

        public string GetQuery();
    }
}