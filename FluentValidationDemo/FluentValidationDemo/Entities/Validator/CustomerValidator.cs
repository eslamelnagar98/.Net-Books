using FluentValidation;
namespace FluentValidationDemo.Entities.Validator;
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(customer => customer.Surname).NotEmpty();
        RuleFor(customer => customer.Address).SetValidator(new AddressValidator());
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