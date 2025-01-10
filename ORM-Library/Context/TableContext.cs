using System;
using System.ComponentModel;
using System.Reflection;

public class Table<T> where T : class
{

    private DatabaseConnection _databaseConnection;

    public Table (DatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

// get, update
    public bool Add<Y>(Y argument)
    {

        if(typeof(T).Equals(typeof(Y)))
        {
            return false;
        }

        PropertyInfo[] properties = typeof(Y).GetProperties();

        var insertQuerryBuilder = new InsertQuerryBuilder();
        string querry, columns = "", values = "", table = typeof(Y).Name;

        foreach(PropertyInfo property in properties)
        {
            columns += property.Name + ", ";
            values +=  $"'{property.GetValue(argument)}'";
        }

        columns = columns.TrimEnd(',', ' ');
        values = values.TrimEnd(',', ' ');
        List<string> valuesList = values.Split(new[] {", "}, StringSplitOptions.None).ToList(); //przeksztalca string values na liste



        insertQuerryBuilder.BuildInsertInto(table, columns).BuildValues(valuesList);
        querry = insertQuerryBuilder.GetQuery();
        var querryExecutor = _databaseConnection.CreateQueryExecutor();
        querryExecutor.ExecuteQuery(querry);
        
        return true;
    }

    public bool Remove<Y>(Y argument)
    {
        if(typeof(T).Equals(typeof(Y)))
        {
            return false;
        }

        PropertyInfo[] properties = typeof(Y).GetProperties();

        var deleteQuerryBuilder = new DeleteQuerryBuilder();
        string querry, columns = "", values = "", table = typeof(Y).Name;

        foreach(PropertyInfo property in properties)
        {
            columns += property.Name + ", ";
            values +=  $"'{property.GetValue(argument)}'";
        }

        columns = columns.TrimEnd(',', ' ');
        values = values.TrimEnd(',', ' ');
        List<string> columnsList = columns.Split(new[] {", "}, StringSplitOptions.None).ToList();
        List<string> valuesList = values.Split(new[] {", "}, StringSplitOptions.None).ToList();

        string condition = "";
        for(int i = 0; i < valuesList.Count; i++)
        {
            if(i == valuesList.Count - 1)
            {
                condition += columnsList[i] + " = " + valuesList[i];
                break;
            }
            condition += columnsList[i] + " = " + valuesList[i] + " AND ";
        }

        deleteQuerryBuilder.BuildDeleteFrom(table).BuildWhere(condition);
        querry = deleteQuerryBuilder.GetQuery();
        var querryExecutor = _databaseConnection.CreateQueryExecutor();
        querryExecutor.ExecuteQuery(querry);

        return true;
    }


    

}
