using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMTest.Domain
{
    [Table("workers")]
    public class Worker
    {
        [Key]
        public int Worker_id {get; set;}
        [ForeignKey("companies")]
        public int Company_id {get; set;}
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(12)]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        [MaxLength(10)]
        public string Age { get; set; }

        //public int Salary {get; set;}

    }
}