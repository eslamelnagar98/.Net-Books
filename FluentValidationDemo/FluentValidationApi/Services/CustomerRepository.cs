namespace FluentValidationApi.Services;
public class CustomerRepository : ICustomerRepository
{
    private readonly FluentValidationContext _fluentValidationContext;

    public CustomerRepository(FluentValidationContext fluentValidationContext)
    {
        _fluentValidationContext = fluentValidationContext;
    }

    public async Task AddCustomer(Customer customer)
    {
        await _fluentValidationContext.Customers.AddAsync(customer);
        await _fluentValidationContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyCollection<Customer>> GetAllCustomers()
    {
        return await _fluentValidationContext.Customers.ToListAsync();
    }

    public async Task<Customer> GetById(int id)
    {
        return (await GetAllCustomers())
            .SingleOrDefault(customer => customer.Id == id);
    }
}
