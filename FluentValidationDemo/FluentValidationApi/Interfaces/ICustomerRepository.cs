namespace FluentValidationApi.Interfaces;
public interface ICustomerRepository
{
    Task AddCustomer(Customer customer);
    Task<IReadOnlyCollection<Customer>> GetAllCustomers();
    Task<Customer> GetById(int id);
}
