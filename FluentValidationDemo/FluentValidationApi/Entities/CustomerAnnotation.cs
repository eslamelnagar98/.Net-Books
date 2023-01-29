using System.ComponentModel.DataAnnotations.Schema;

namespace FluentValidationApi.Entities;
public class CustomerAnnotation
{
    public int Id { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public string Forename { get; set; }
    [Required]
    [Range(0.1d, double.MaxValue)]
    public decimal Discount { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public AddressAnnotation Address { get; set; }

    [NotMapped]
    public List<string> Basket { get; set; } = new();

}
