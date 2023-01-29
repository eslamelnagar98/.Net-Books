namespace FluentValidationApi.Entities.Validator;
public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {

        RuleFor(address => address.Town)
           .NotEmpty();

        RuleFor(address => address.Postcode)
           .NotEmpty();

        RuleFor(address => address.Line2)
           .NotEmpty();

        RuleFor(address => address.County)
           .NotEmpty();

        RuleFor(address => address.Line1)
           .NotEmpty();
    }
}
