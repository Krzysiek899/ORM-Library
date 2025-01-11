namespace ORMLibrary.Mapping
{
    public class DatabaseMapping
    {
        public List<TableMapping> Tables {get;} = new();

        // private readonly List<RelationMapping> relations = new();

        // public void AddRelation(RelationMapping relation){
        //     relations.Add(relation);
        // }

        public void AddTable(TableMapping table)
        {
            Tables.Add(table);
        }

    }

}