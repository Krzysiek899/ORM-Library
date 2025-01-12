using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMTest.Domain
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        public int Customer_id {get; set;}
        [ForeignKey("orders")]
        public int Cusomer_order_id {get; set;}

        [MaxLength(64)]
        public string Email {get; set;}
        [MaxLength(64)]
        public string Name {get; set;}
        public string Adress {get; set;}
    }
}