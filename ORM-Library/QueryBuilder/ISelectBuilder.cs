public interface ISelectBuilder
{
    ISelectBuilder BuildSelect(string columns);
    ISelectBuilder BuildFrom(string tables);
    ISelectBuilder BuildJoin(string tables);
    ISelectBuilder BuildWhere(string condition);
    ISelectBuilder GroupBy();
    ISelectBuilder Having();
    
}