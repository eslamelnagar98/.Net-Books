namespace FluentValidationApi.Entities;
public class Customer
{
    public int Id { get; set; }
    public string Surname { get; set; }
    public string Forename { get; set; }
    public decimal Discount { get; set; }
    public string Email { get; set; }
    public List<string> Basket { get; set; } = new();
    public CustomerStatus customerStatus { get; set; }
    public Address Address { get; set; }
}
