using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
public class Table{
    

    public Table(string name, List<Column> columns, List<Key> primaryKeys, List<Key> foreignKeys){
        Name = name;
        Columns = columns;
        PrimaryKeys = primaryKeys;
        ForeignKeys = foreignKeys;
    }

    public required string Name {get; set;}
    public required List<Column> Columns {get; set;}

    public required List<Key> PrimaryKeys {get; set;}
    public  List<Key> ForeignKeys {get; set;}

    public bool AddColumn(Column column) {

        if(Columns.Any(c => c.Name == column.Name))
        {
            throw new Exception("Column already exists");
        }
        Columns.Add(column);
        return true;
    }

    public bool RemoveColumn(string columnName) {
        if(Columns.Any(c => c.Name == columnName))
        {
            Columns.Remove(Columns.First(c => c.Name == columnName));
            return true;
        }
        return false;
    }

    public bool AddForeignKey(Key foreignKey) {
        if(ForeignKeys.Any(fk => fk.Name == foreignKey.Name))
        {
            throw new Exception("Foreign Key already exists");
        }
        ForeignKeys.Add(foreignKey);
        return true;
    }

    public bool AddPrimaryKey(Key primaryKey) {
        if(PrimaryKeys.Any(fk => fk.Name == primaryKey.Name))
        {
            throw new Exception("Primary Key already exists");
        }
        PrimaryKeys.Add(primaryKey);
        return true;
    }

    

    
}