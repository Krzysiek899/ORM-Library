
public class Column
{

    public Column(string name, ColumnType type){
        Name = name;
        Type = type;
    }

    public string Name {get; set;}
    public ColumnType Type {get; set;}

}