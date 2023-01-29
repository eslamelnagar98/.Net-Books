namespace FluentValidationApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IValidator<Customer> _validator;
    private readonly ICustomerRepository _customerRepository;
    private List<Customer> _customers = new();

    public CustomerController(IValidator<Customer> validator, ICustomerRepository customerRepository)
    {
        _validator = validator;
        _customerRepository = customerRepository;
    }

    [HttpPost]
    public async Task<ActionResult> AddCustomer(Customer customer)
    {
        var result = await _validator.ValidateAsync(customer);
        if (result.IsValid is false)
        {
            return BadRequest(result);
        }
        await _customerRepository.AddCustomer(customer);
        return Ok();
    }

}
