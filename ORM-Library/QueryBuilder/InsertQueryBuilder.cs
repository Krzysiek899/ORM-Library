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

        public IInsertBuilder BuildValues(List<string> values)
        {
            string valuesString = "";
            for(int i = 0; i < values.Count; i++)
            {
                if(values.Count == 1)
                {
                    valuesString += values[i] + "\n";
                    break;
                }
                if(i == values.Count - 1)
                {
                    valuesString += values[i] + "\n";
                    break;
                }
                valuesString += values[i] + ",\n"; 
            }
            _insertQuerry += "VALUES " + valuesString;
            return this;
        }

        public string GetQuery(){
            return _insertQuerry + ";";
        }

    }
}