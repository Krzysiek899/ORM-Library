using System.ComponentModel.DataAnnotations;

public class Company : Entity {

    [Key]
    public int Company_id {get; set;}
    public string Name {get; set;}
    public string Address {get; set;}

}