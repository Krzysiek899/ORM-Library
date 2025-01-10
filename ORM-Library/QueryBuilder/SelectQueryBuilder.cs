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

    public ISelectBuilder Having()
    {
        _selectQuerry += "HAVING " + 
    }

    public string GetQuerry(){
        return _selectQuerry;
    }

}