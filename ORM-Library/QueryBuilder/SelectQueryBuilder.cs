public class SelectQuerryBuilder : ISelectBuilder
{
    private string _selectQuerry = "";

    public ISelectBuilder BuildSelect(string columns)
    {
        _selectQuerry += "SELECT " + columns + "\n";
        return this;
    }

    public ISelectBuilder BuildFrom(string tables)
    {
        _selectQuerry += "FROM " + tables + "\n";
        return this;
    }

    public ISelectBuilder BuildJoin(string tables)
    {
        _selectQuerry += "JOIN " + tables + "\n";
        return this;
    }

    public ISelectBuilder BuildWhere(string condition)
    {
        _selectQuerry += "WHERE " + condition + "\n"; 
        return this;
    }

    public ISelectBuilder GroupBy(string columns)
    {
        _selectQuerry += "GROUP BY " + columns + "\n";
        return this;
    }

    public ISelectBuilder Having(string condition)
    {
        _selectQuerry += "HAVING " + condition;
        return this;
    }

    public ISelectBuilder BuildOrderByASC(string columns)
    {
        _selectQuerry += "ORDER BY " + columns + " ASC\n";
        return this;
    }

    public ISelectBuilder BuildOrderByDESC(string columns)
    {
        _selectQuerry += "ORDER BY " + columns + " DESC\n";
        return this;
    }

    public ISelectBuilder BuildLimit(int limit)
    {
        _selectQuerry += "LIMIT " + limit + "\n";
        return this;
    }

    public ISelectBuilder BuildOffset(int offset)
    {
        _selectQuerry += "OFFSET " + offset + "\n";
        return this;
    }

    public string GetQuery(){
        return _selectQuerry + ";";
    }

}