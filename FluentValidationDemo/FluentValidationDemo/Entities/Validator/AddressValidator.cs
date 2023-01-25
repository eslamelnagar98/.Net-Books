using FluentValidation;

namespace FluentValidationDemo.Entities.Validator;
public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(address => address.Postcode).NotNull();
        //etc
    }
}
