public class Column
{

    public Column(string name, PropertyType type){
        Name = name;
        Type = type;
    }

    public string Name {get; set;}
    public PropertyType Type {get; set;}

}