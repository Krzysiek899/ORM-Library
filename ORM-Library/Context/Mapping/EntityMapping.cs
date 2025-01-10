public class TableMapping{
    public string? TableName { get; set; }
    public string? PrimaryKey { get; set; }

    public List<PropertyMapping> Property {get; set;} = new();
    public List<RelationMapping> Relations { get; set; } = new();
}