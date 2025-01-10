public interface IUpdateBuilder
{
    IUpdateBuilder BuildUpdate(string tables);
    IUpdateBuilder BuildJoin(string tables);
    IUpdateBuilder BuildSet(string column, string value);
    IUpdateBuilder BuildWhere(string condition);
    IUpdateBuilder BuildOrderByASC(string columns);
    IUpdateBuilder BuildOrderByDESC(string columns);
    IUpdateBuilder BuildLimit(int limit);
    IUpdateBuilder BuildOffset(int offset);
    
}
