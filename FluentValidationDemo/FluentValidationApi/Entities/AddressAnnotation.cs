using System.ComponentModel.DataAnnotations;

namespace FluentValidationApi.Entities;
public class AddressAnnotation
{
    [Required]
    public string Line1 { get; set; }
    [Required]

    public string Line2 { get; set; }
    [Required]

    public string Town { get; set; }
    [Required]

    public string County { get; set; }
    [Required]

    public string Postcode { get; set; }
}
