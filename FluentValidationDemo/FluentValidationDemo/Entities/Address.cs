using System.ComponentModel.DataAnnotations;

namespace FluentValidationDemo.Entities;
public class Address
{
    public string Line1 { get; set; }
    public string Line2 { get; set; }
    public string Town { get; set; }
    public string County { get; set; }
    [Required]
    public string Postcode { get; set; }
}
