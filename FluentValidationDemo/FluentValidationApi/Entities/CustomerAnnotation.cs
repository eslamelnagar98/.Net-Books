using System.ComponentModel.DataAnnotations;
namespace FluentValidationApi.Entities;
public class CustomerAnnotation
{
    [Required]
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

    [Required]
    public List<string> Basket { get; set; } = new();

}
