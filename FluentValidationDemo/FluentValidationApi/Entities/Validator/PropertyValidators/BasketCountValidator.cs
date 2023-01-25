using FluentValidation;
using FluentValidation.Validators;

namespace FluentValidationApi.Entities.Validator.PropertyValidators;
public class BasketCountValidator : PropertyValidator<Customer, List<string>>
{
    public override string Name => nameof(BasketCountValidator);
    private short _maxCount;
    public BasketCountValidator(short maxCount)
    {
        _maxCount = maxCount;
    }
    public override bool IsValid(ValidationContext<Customer> context, List<string> basketList)
    {
        if (basketList is not null && basketList.Count < _maxCount)
        {
            return true;
        }

        return false;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
        => $"basket List must contain fewer than {_maxCount} items.";
}
