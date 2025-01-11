namespace ORMLibrary.QueryBuilders
{
    public class AlterQuerryBuilder : IAlterBuilder
    {
        private string _alterQuery = "";
        public IAlterBuilder BuildAlterTable(string tables)
        {
            _alterQuery += "ALTER TABLE " + tables + "\n";
            return this;
        }
        
        public IAlterBuilder BuildAddColumn(string column, string type)
        {
            _alterQuery += "ADD COLUMN " + column + " " + type + "\n";
            return this;
        }

        public IAlterBuilder BuildDropColumn(string column)
        {
            _alterQuery += "DROP COLUMN " + column + "\n";
            return this;
        }

        public IAlterBuilder BuildModifyColumn(string column, string type)
        {
            _alterQuery += "MODIFY COLUMN " + column + " " + type + "\n";
            return this;
        }

        public IAlterBuilder BuildChangeColumn(string column, string newColumn, string type)
        {
            _alterQuery += "CHANGE COLUMN " + column + " " + newColumn + " " + type + "\n";
            return this;
        }

        public IAlterBuilder BuildRenameTable(string table)
        {
            _alterQuery += "RENAME TO " + table + "\n";
            return this;
        }

        public IAlterBuilder BuildRenameColumn(string column, string newColumn)
        {
            _alterQuery += "RENAME COLUMN " + column + " TO " + newColumn + "\n";
            return this;
        }

        public IAlterBuilder BuildAddPrimaryKey(string column)
        {
            _alterQuery += "ADD PRIMARY KEY(" + column + ")\n";
            return this;
        }

        public IAlterBuilder BuildAddConstraint(string key)
        {
            _alterQuery += "ADD CONSTRAINT " + key + "\n";
            return this;
        }

        public IAlterBuilder BuildForeignKey(string key)
        {
            _alterQuery += "FOREIGN KEY (" + key + ")\n";
            return this;
        }

        public IAlterBuilder BuildReferences(string targetTable, string targetColumn)
        {
            _alterQuery += "REFERENCES " + targetTable + " (" + targetColumn + ")";
            return this;
        }

        public IAlterBuilder BuildFinal()
        {
            _alterQuery += ";";
            return this;
        }

        public string GetQuery()
        {
            return _alterQuery;
        }

        
    }
}