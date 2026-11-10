using FluentValidation;

namespace ShipNotes.Application.Common;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string?> MustBeAbsoluteHttpUrl<T>(this IRuleBuilder<T, string?> rule) =>
        rule.Must(value => string.IsNullOrWhiteSpace(value)
                || (Uri.TryCreate(value, UriKind.Absolute, out var uri)
                    && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp)))
            .WithMessage("'{PropertyName}' must be an absolute http or https URL.");

    public static IRuleBuilderOptions<T, string> MustBeSemanticVersion<T>(this IRuleBuilder<T, string> rule) =>
        rule.Must(value => Domain.Versioning.SemanticVersion.TryParse(value, out _))
            .WithMessage("'{PropertyName}' must be a semantic version, e.g. 1.4.0 or 2.0.0-beta.1.");
}
