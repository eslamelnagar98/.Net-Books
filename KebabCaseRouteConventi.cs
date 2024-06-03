
services
    .AddControllers(options => { options.Conventions.Add(new KebabCaseRouteConvention()); })

public class KebabCaseRouteConvention : IApplicationModelConvention
{
    public void Apply(Microsoft.AspNetCore.Mvc.ApplicationModels.ApplicationModel application)
    {

        application.Controllers?.SelectMany(controller => controller?.Actions ?? new List<ActionModel>(0))
                               ?.SelectMany(action => action?.Selectors ?? new List<SelectorModel>(0))
                               ?.ForEach(selector =>
                                {
                                    if (selector?.AttributeRouteModel?.Template is not null)
                                    {
                                        selector.AttributeRouteModel.Template = ToKebabCase(selector.AttributeRouteModel.Template);
                                    }
                                });
    }

    private string ToKebabCase(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input ?? string.Empty;
        }

        var kebabCase = Regex.Replace(input, "([a-z])([A-Z])", "$1-$2").ToLower();

        return kebabCase.Replace("_", "-");
    }
}