using FluentValidation;

namespace JobTrackingAPI.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    protected BaseValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;
    }

    protected bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    protected bool BeAValidDate(DateTime date)
    {
        return date != default;
    }

    protected bool BeAValidDate(DateTime? date)
    {
        return date != null && date != default(DateTime);
    }
}
