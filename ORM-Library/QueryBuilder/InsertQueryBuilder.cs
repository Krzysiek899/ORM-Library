namespace ORMLibrary.QueryBuilders
{
    public class InsertQuerryBuilder : IInsertBuilder
    {
        private string _insertQuerry = "";

        public IInsertBuilder BuildInsertInto(string table, string? columns)
        {
            _insertQuerry += "INSERT INTO " + table + " ";
            if(columns is not null)
            {
                _insertQuerry += "(" + columns + " )" + "\n";
            } 
            return this;
        }

        public IInsertBuilder BuildValues(string values)
        {
            
            // // Złączenie wartości z przecinkami, dodając przecinek po każdej wartości
            // foreach (var value in values){
            //     values[values.IndexOf(value)] = "'" + value + "'";
            // }
            // string valuesString = string.Join(", ", values);

            // Składa finalne zapytanie
            _insertQuerry += "VALUES (" + values + ")";
            return this;
            
        }

        public string GetQuery(){
            return _insertQuerry + ";";
        }

    }
}