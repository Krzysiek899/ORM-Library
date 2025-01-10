public class DeleteQuerryBuilder : IDeleteBuilder
{
    private string _deleteQuery = "";
    public IDeleteBuilder BuildDeleteFrom(string tables)
    {
        _deleteQuery += "DELETE FROM " + tables + "\n";
        return this;
    }
    public IDeleteBuilder BuildWhere(string condition)
    {
        _deleteQuery += "WHERE " + condition + "\n";
        return this;
    }
    public IDeleteBuilder BuildOrderByASC(string columns)
    {
        _deleteQuery += "ORDER BY " + columns + "ASC\n";
        return this;
    }
    public IDeleteBuilder BuildOrderByDESC(string columns)
    {
        _deleteQuery += "ORDER BY " + columns + "DESC\n";
        return this;
    }
    public IDeleteBuilder BuildLimit(int limit)
    {
        _deleteQuery += "LIMIT " + limit + "\n";
        return this;
    }
    public string GetQuery()
    {
        return _deleteQuery + ";";
    }

}