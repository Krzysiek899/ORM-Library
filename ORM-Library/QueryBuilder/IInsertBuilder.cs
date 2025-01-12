namespace ORMLibrary.QueryBuilders
{
    public interface IInsertBuilder
    {
        public IInsertBuilder BuildInsertInto(string table, string? columns);   
        public IInsertBuilder BuildValues(string values);
    }
}