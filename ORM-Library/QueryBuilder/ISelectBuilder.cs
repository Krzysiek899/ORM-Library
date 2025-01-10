namespace QueryBuilders
{
    public interface ISelectBuilder
    {
        ISelectBuilder BuildSelect(string columns);
        ISelectBuilder BuildFrom(string tables);
        ISelectBuilder BuildJoin(string tables);
        ISelectBuilder BuildWhere(string condition);
        ISelectBuilder GroupBy(string columns);
        ISelectBuilder Having(string condition);
        ISelectBuilder BuildOrderByASC(string columns);
        ISelectBuilder BuildOrderByDESC(string columns);
        ISelectBuilder BuildLimit(int limit);
        ISelectBuilder BuildOffset(int offset);
        
    }
}

