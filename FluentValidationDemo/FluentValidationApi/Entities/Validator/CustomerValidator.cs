using FluentValidation;

namespace FluentValidationApi.Entities.Validator;
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(customer => customer.Id)
            .NotEmpty();
        RuleFor(customer => customer.Surname)
            .NotEmpty()
            .Length(3, 250);
        RuleFor(customer => customer.Forename)
            .NotEmpty()
            .Length(customer => customer.Surname.Length - 2, customer => customer.Surname.Length + 2);
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
//public class UserEditSelfResourceValidator : AbstractValidator<UserEditSelfResource>
//{
//    private IUserRepository _userRepository;

//    public UserEditSelfResourceValidator(IUserRepository userRepository, ILoggedInUserService loggedInUser)
//    {
//        _userRepository = userRepository;
//        RuleFor(mem => mem.ProfileUrl).MustAsync(async (entity, value, c) => await UniqueProfileUrl(entity, value))
//            .WithMessage("Profile Url must be unique.");

//    }

//    public async Task<bool> UniqueProfileUrl(UserEditSelfResource userEditSelf, string newProfileUrl)
//    {
//        var user = await _userRepository.FindByProfileUrlAsync(newProfileUrl);

//        if (user.Id == _loggedInUser.GetId() || user == null)
//        {
//            return true;
//        }
//        return false;
//    }
//}