namespace FluentValidationApi.Entities.Validator.PropertyValidators;
public class CustomerSurnameValidator : AsyncPropertyValidator<Customer, string>
{
    private readonly ICustomerRepository _customerRepository;
    private string _surnameValue;
    private const int MinimumLength = 3;
    private const int MaximumLength = 15;
    private string _lengthValidationErrorMessage = string.Empty;
    public CustomerSurnameValidator(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    public override string Name => nameof(CustomerSurnameValidator);

    public override async Task<bool> IsValidAsync(ValidationContext<Customer> context, string value, CancellationToken cancellation)
    {
        _surnameValue = value;
        if (value is null) return false;

        return IsCustomerSurnameLengthMatchTheRange(context, value)
            ? await IsCustomerSurnameAlreadyExist(value)
            : default;

    }
    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return _surnameValue is null ? "Surname Cannot Be Null" : _lengthValidationErrorMessage;
    }

    private async Task<bool> IsCustomerSurnameAlreadyExist(string customerSurname)
    {
        var customerList = await _customerRepository.GetAllCustomers();
        var isCustomerListContainsNewCustomerSurname = customerList.Any(customer => customer.Surname == customerSurname);
        if (isCustomerListContainsNewCustomerSurname)
            return false;
        return true;
    }

    private bool IsCustomerSurnameLengthMatchTheRange(ValidationContext<Customer> context, string customerSurname)
    {
        if (customerSurname.Length is >= MinimumLength and <= MaximumLength)
        {
            return true;
        }

        var totalLength = customerSurname.Length;
        context.MessageFormatter
            .AppendArgument("Minimum", MinimumLength)
            .AppendArgument("Maximum", MaximumLength)
            .AppendArgument("TotalLength", totalLength);
        _lengthValidationErrorMessage = $"Customer Surname Must Be Between {MinimumLength} And {MaximumLength}";
        return default;
    }

    private void AddNewFailure(ValidationContext<Customer> context, string customerSurname)
    {
        var validationFailure = new ValidationFailure
        {
            PropertyName = "Customer Surname",
            ErrorMessage = $"Customer Surname Length Must Be Between {MinimumLength} And {MaximumLength}",
            AttemptedValue = customerSurname,
            ErrorCode = Name,

        };
        context.AddFailure(validationFailure);
    }
}
