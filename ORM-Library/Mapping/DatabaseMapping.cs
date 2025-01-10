public class DataBaseMapping{
    private readonly List<TableMapping> tables = new();

    // private readonly List<RelationMapping> relations = new();

    // public void AddRelation(RelationMapping relation){
    //     relations.Add(relation);
    // }

    public void AddTable(TableMapping table){
        tables.Add(table);
    }

}