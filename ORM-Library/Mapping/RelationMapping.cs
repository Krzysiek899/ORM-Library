namespace Mapping
{

    public class RelationMapping
    {
        public RelationType RelationType {get; set;}
        public TableMapping FirstTable {get; set;}

        public TableMapping SecondTable {get; set;}

        public RelationMapping(RelationType relationType, TableMapping first, TableMapping second){
            RelationType = relationType;
            FirstTable = first;
            SecondTable = second;
        }
    }

    public enum RelationType{
        OneToOne,
        ManyToMany,
        OneToMany
    }

}