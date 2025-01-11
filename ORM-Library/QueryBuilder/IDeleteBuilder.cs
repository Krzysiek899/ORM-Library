namespace ORMLibrary.QueryBuilders
{
    public interface IDeleteBuilder
    {
        IDeleteBuilder BuildDeleteFrom(string tables);
        IDeleteBuilder BuildWhere(string condition);
        IDeleteBuilder BuildOrderByASC(string column);
        IDeleteBuilder BuildOrderByDESC(string column);
        IDeleteBuilder BuildLimit(int limit);
        
    }
}