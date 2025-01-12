using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORMTest.Domain
{
    [Table("orders")]
    public class Orders
    {
        [Key]
        public int Order_id {get; set;}
        [ForeignKey("products")]
        public int Product_id {get; set;}
        [ForeignKey("companies")]
        public int Company_id {get; set;}

        public bool Status {get; set;}

    }
}