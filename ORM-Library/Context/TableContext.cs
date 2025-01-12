using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using ORMLibrary.DataAccess;
using ORMLibrary.QueryBuilders;
using ORMLibrary.Iterator;

namespace ORMLibrary.Context{

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

            var insertQueryBuilder = new InsertQuerryBuilder();
            string query, columns = "", values = "", table = typeof(Y).Name;

            foreach(PropertyInfo property in properties)
            {
                columns += property.Name + ", ";
                values +=  $"'{property.GetValue(argument)}'";
            }

            columns = columns.TrimEnd(',', ' ');
            values = values.TrimEnd(',', ' ');
            List<string> valuesList = values.Split(new[] {", "}, StringSplitOptions.None).ToList(); //przeksztalca string values na liste

            insertQueryBuilder.BuildInsertInto(table, columns).BuildValues(valuesList);
            query = insertQueryBuilder.GetQuery();
            var queryExecutor = _databaseConnection.CreateQueryExecutor();
            queryExecutor.ExecuteNonQuery(query);
            
            return true;
        }

        public bool Remove<Y>(Y argument)
        {
            if(typeof(T).Equals(typeof(Y)))
            {
                return false;
            }

            PropertyInfo[] properties = typeof(Y).GetProperties();

            var deleteQueryBuilder = new DeleteQuerryBuilder();
            string query, columns = "", values = "", table = typeof(Y).Name;

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

            deleteQueryBuilder.BuildDeleteFrom(table).BuildWhere(condition);
            query = deleteQueryBuilder.GetQuery();
            var queryExecutor = _databaseConnection.CreateQueryExecutor();
            queryExecutor.ExecuteNonQuery(query);

            return true;
        }

        public List<T> ToList()
        {   
            var selectQuerryBuilder = new SelectQuerryBuilder();
            selectQuerryBuilder.BuildSelect("*").BuildFrom(typeof(T).Name);
            string query = selectQuerryBuilder.GetQuery();
            var queryExecutor = _databaseConnection.CreateQueryExecutor();
            DataTable result = queryExecutor.ExecuteQuery(query);

            var entityCollection = new EntityCollection();
            foreach(DataRow row in result.Rows)
            {
                entityCollection.AddItem(row);
            }
            var entityIterator = new EntityIterator(entityCollection, false);

            var list = new List<T>();

            while(entityIterator.HasNext())
            {
                T obj = Activator.CreateInstance<T>();

                DataRow row = (DataRow)entityIterator.Current();

                foreach(PropertyInfo property in typeof(T).GetProperties())
                {   
                    if(result.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    {
                        property.SetValue(obj, row[property.Name]);
                    }
                }
                list.Add(obj);
                entityIterator.MoveNext();
            }
            return list;
        }   

        public T? First()
        {
            var list = ToList();
            return list.Count > 0 ? list[0] : default(T);
        } 


        public bool Update<Y>(Y argument)
        {

            return true;
        }

    }
}