using System.ComponentModel.DataAnnotations;

namespace FluentValidationDemo.Entities;
public class CustomerAnnotation
{
    public int Id { get; set; }
    [Required]
    public string Surname { get; set; }
    public string Forename { get; set; }
    public decimal Discount { get; set; }
    public Address Address { get; set; }
}
