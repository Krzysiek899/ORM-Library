namespace ORMLibrary.Mapping
{
    public class ForeignKey
    {
        public string Name {get; set;}
        public string ReferenceTable {get; set;}

        public ForeignKey(string name, string referenceTable)
        {
            Name = name;
            ReferenceTable = referenceTable;
        }
    }

}