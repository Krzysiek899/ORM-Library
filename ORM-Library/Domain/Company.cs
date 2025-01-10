using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("Companies")]
public class Company
{
    [Key]
    public int Company_id {get; set;}
    public string Name {get; set;}
    public string Address {get; set;}
}