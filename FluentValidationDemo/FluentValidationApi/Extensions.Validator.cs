using FluentValidation;
using FluentValidationApi.Entities;
using FluentValidationApi.Entities.Validator.PropertyValidators;
namespace FluentValidationApi;
public static partial class Extensions
{
    public static IRuleBuilderOptions<Customer, List<string>> BasketMustContainFewerThan(this IRuleBuilder<Customer, List<string>> ruleBuilder,
                                                                                         short maxCount)
    {
        return ruleBuilder.SetValidator(new BasketCountValidator(maxCount));
    }

}
