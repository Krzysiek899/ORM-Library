using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMTest.Domain
{
    [Table("Workers")]
    public class Worker
    {
        [Key]
        public int Worker_id {get; set;}
        [ForeignKey("Companies")]
        public int Company_id {get; set;}
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public int Salary {get; set;}

    }
}