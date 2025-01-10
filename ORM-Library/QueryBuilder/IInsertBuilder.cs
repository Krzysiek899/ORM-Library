namespace QueryBuilders
{
    public interface IInsertBuilder
    {
        public IInsertBuilder BuildInsertInto(string table, string? columns);   
        public IInsertBuilder BuildValues(List<string> values);
    }
}