using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using ORMLibrary.DataAccess;
using ORMLibrary.QueryBuilders;
using ORMLibrary.Iterator;
using System.ComponentModel.DataAnnotations.Schema;
using ORMLibrary.Logging;
using System.ComponentModel.DataAnnotations;

namespace ORMLibrary.Context{

    public class Table<T> where T : class
    {
        private readonly Logger logger = Logger.GetInstance();
        private DatabaseConnection _databaseConnection;

        private DataTable data;

        public Table (DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
            LoadData();
        }

        public string GetTableName()
        {
            var tableAttribute = typeof(T).GetCustomAttribute<TableAttribute>();

            if (tableAttribute != null && !string.IsNullOrWhiteSpace(tableAttribute.Name))
            {
                return tableAttribute.Name;
            }

            return typeof(T).Name;
        }
        public void LoadData()
        { 
            var selectQuerryBuilder = new SelectQuerryBuilder();
            selectQuerryBuilder.BuildSelect("*").BuildFrom(this.GetTableName()); 
            string query = selectQuerryBuilder.GetQuery();
            var queryExecutor = _databaseConnection.CreateQueryExecutor();
            data = queryExecutor.ExecuteQuery(query);
        }

    // get, update
        public bool Add<Y>(Y argument)
        {

            if(!typeof(T).Equals(typeof(Y)))
            {
                return false;
            }

            PropertyInfo[] properties = typeof(Y).GetProperties();

            var insertQueryBuilder = new InsertQuerryBuilder();
            string query, columns = "", values = "", table = GetTableName();

            foreach(PropertyInfo property in properties)
            {
                columns += property.Name + ", ";
                values +=  $"'{property.GetValue(argument)}', ";
            }

            columns = columns.TrimEnd(',', ' ');
            values = values.TrimEnd(',', ' ');

            insertQueryBuilder.BuildInsertInto(table, columns).BuildValues(values);
            query = insertQueryBuilder.GetQuery();
         
            logger.LogInfo(query);

            var queryExecutor = _databaseConnection.CreateQueryExecutor();
            queryExecutor.ExecuteNonQuery(query);
            
            LoadData();
            return true;
        }

        public bool Remove<Y>(Y argument)
        {
            if(!typeof(T).Equals(typeof(Y)))
            {
                return false;
            }

            PropertyInfo[] properties = typeof(Y).GetProperties();

            var deleteQueryBuilder = new DeleteQuerryBuilder();
            string query, columns = "", values = "", table = GetTableName();

            foreach(PropertyInfo property in properties)
            {
                if(property.GetValue(argument) is (object?)"" or null)
                {
                    continue;
                }
                columns += property.Name + ", ";
                values +=  $"'{property.GetValue(argument)}', ";
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
            logger.LogInfo(query);

            var queryExecutor = _databaseConnection.CreateQueryExecutor();
            queryExecutor.ExecuteNonQuery(query);

            LoadData();

            return true;
        }

        public List<T> ToList()
        {   
            data = data ?? new DataTable();

            var entityCollection = new EntityCollection();
            foreach(DataRow row in data.Rows)
            {
                entityCollection.AddItem(row);
            }
            var entityIterator = new EntityIterator(entityCollection, false);

            var list = new List<T>();

            while(entityIterator.MoveNext())
            {
                T obj = Activator.CreateInstance<T>();

                DataRow row = (DataRow)entityIterator.Current();

                foreach(PropertyInfo property in typeof(T).GetProperties())
                {   
                    if(data.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    {
                        property.SetValue(obj, row[property.Name]);
                    }
                }
                list.Add(obj);
            }

            LoadData();

            return list;
        }     

        public T? First()
        {
            var list = ToList();
            if (list.Count == 0 || list == null)
            {
                return null;
            }
            return list[0];
        }


        public bool Update<Y>(Y argument)
        {
            if(!typeof(T).Equals(typeof(Y)))
            {
                return false;
            }
            PropertyInfo[] properties = typeof(Y).GetProperties();
            var updateQueryBuilder = new UpadateQuerryBuilder();
            string query, setClause = "", whereClause = "", table = GetTableName();

            foreach(PropertyInfo property in properties)
            {
                var value = property.GetValue(argument);
                if(value is (object?)"" or null)
                {
                    continue;
                }

                if (property.GetCustomAttribute<KeyAttribute>() != null)
                {
                    whereClause += $"{property.Name} = '{value}' AND ";
                }
                else
                {
                    setClause += $"{property.Name} = '{value}', ";
                }
            }
            
            setClause = setClause.TrimEnd(',', ' ');
            whereClause = whereClause.TrimEnd(',', ' ', 'A', 'N', 'D');
            
            if(string.IsNullOrEmpty(setClause) || string.IsNullOrEmpty(whereClause))
            {
                return false;
            }
                            
            updateQueryBuilder.BuildUpdate(table).BuildSet(setClause).BuildWhere(whereClause);
            query = updateQueryBuilder.GetQuery();

            logger.LogInfo(query);

            var queryExecutor = _databaseConnection.CreateQueryExecutor();
            queryExecutor.ExecuteNonQuery(query);

            LoadData();
            return true;
        }

        public Table<T> Include(Func<T, bool> predicate)
        {
            data = data ?? new DataTable();

            var entityCollection = new EntityCollection();

            foreach (DataRow row in data.Rows)
            {
                entityCollection.AddItem(row);
            }

            var entityIterator = new EntityIterator(entityCollection, false);

            var updatedData = data.Clone();  

            while (entityIterator.MoveNext())
            {
                var row = (DataRow)entityIterator.Current();
                T obj = Activator.CreateInstance<T>();

                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    if (data.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    {
                        property.SetValue(obj, row[property.Name]);
                    }
                }

                if (predicate(obj))
                {
                    updatedData.ImportRow(row);
                }
            }

            data = updatedData;

            return this;
        }
    }
}