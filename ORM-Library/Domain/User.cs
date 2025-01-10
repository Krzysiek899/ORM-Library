using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("Users")]
public class User
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

}