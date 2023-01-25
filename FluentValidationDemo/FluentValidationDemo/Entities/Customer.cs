namespace FluentValidationDemo.Entities;
public class Customer
{
    public int Id { get; set; }
    public string Surname { get; set; } = null;
    public string Forename { get; set; }
    public decimal Discount { get; set; }
    public Address Address { get; set; }
}
