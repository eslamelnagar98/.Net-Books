namespace FluentValidationApi.Entities.Validator;
public class CustomerValidator : AbstractValidator<Customer>
{
    private readonly ICustomerRepository _customerRepository;
    public CustomerValidator(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(customer => customer.Surname)
            .SetAsyncValidator(new CustomerSurnameValidator(_customerRepository));
        //.NotEmpty()
        //.Length(3, 250);
        RuleFor(customer => customer.Forename)
            .NotEmpty()
            .Length(customer => customer.Surname.Length, customer => customer.Surname.Length);
        RuleFor(customer => customer.Discount)
            .PrecisionScale(3, 1, false)
            .WithMessage("The Discount Must Contains 3 Precision And 2 Scale Ex. (32.1) ");
        RuleFor(customer => customer.Address)
            .NotEmpty()
            .SetValidator(new AddressValidator());
        RuleFor(customer => customer.Email)
            .EmailAddress();
        RuleFor(customer => customer.customerStatus)
            .IsInEnum();
        RuleFor(customer => customer.Basket)
            .BasketMustContainFewerThan(3);
    }
}
