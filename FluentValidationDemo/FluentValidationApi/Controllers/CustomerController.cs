using FluentValidation;
using FluentValidationApi.Entities;
using Microsoft.AspNetCore.Mvc;
namespace FluentValidationApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IValidator<Customer> _validator;
    private List<Customer> _customers = new();

    public CustomerController(IValidator<Customer> validator)
    {
        _validator = validator;
    }

    [HttpPost]
    public ActionResult AddCustomer(Customer customer)
    {
        _customers.Add(customer);
        return Ok();
    }

}
