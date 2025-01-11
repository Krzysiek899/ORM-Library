using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMTest.Domain
{
    [Table("Companies")]
    public class Company
    {
        [Key]
        public int Company_id {get; set;}

        [MaxLength(100)]
        public string Name {get; set;}
        public string Address {get; set;}
    }
}
