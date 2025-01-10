namespace QueryBuilders
{
    public class UpadateQuerryBuilder : IUpdateBuilder
    {   
        private string _updateQuerry = "";
        public IUpdateBuilder BuildUpdate(string tables)
        {
            // Build Update Query
            _updateQuerry += "UPDATE " + tables + "\n";
            return this;
        }

        public IUpdateBuilder BuildJoin(string tables)
        {
            // Build Join Query
            _updateQuerry += "JOIN " + tables + "\n";
            return this;
        }

        public IUpdateBuilder BuildSet(string columns, string value)
        {
            // Build Set Query
            _updateQuerry += "SET " + columns + " = " + value + "\n";
            return this;
        }

        public IUpdateBuilder BuildWhere(string condition)
        {
            // Build Where Query
            _updateQuerry += "WHERE " + condition + "\n";
            return this;
        }

        public IUpdateBuilder BuildOrderByASC(string columns)
        {
            // Build OrderBy Query
            _updateQuerry += "ORDER BY " + columns + " ASC\n";
            return this;
        }

        public IUpdateBuilder BuildOrderByDESC(string columns)
        {
            // Build OrderBy Query
            _updateQuerry += "ORDER BY " + columns + " DESC\n";
            return this;
        }

        public IUpdateBuilder BuildLimit(int limit)
        {
            // Build Limit Query
            _updateQuerry += "LIMIT " + limit + "\n";
            return this;
        }

        public IUpdateBuilder BuildOffset(int offset)
        {
            // Build Offset Query
            _updateQuerry += "OFFSET " + offset + "\n";
            return this;
        }

        public string GetQuery(){
            return _updateQuerry + ";";
        }

    }
}