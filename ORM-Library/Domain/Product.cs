using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("Products")]
public class Product
{
    [Key]
    public int Product_id {get; set;}
    [ForeignKey("Company")]
    public int Company_id {get; set;}
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category {get; set;}
    

}