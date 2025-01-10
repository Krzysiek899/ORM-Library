public class RelationMapping
{
    public RelationType relation;
    public TableMapping table;
}

public enum RelationType{
    OneToOne,
    ManyToMany,
    OneToMany,
    ManyToOne
}