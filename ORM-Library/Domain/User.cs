using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class User : Entity
{
    [Key]
    public int User_id {get; set;}
    [ForeignKey("Company")]
    public int Company_id {get; set;}
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public Company Company {get; set; }
}