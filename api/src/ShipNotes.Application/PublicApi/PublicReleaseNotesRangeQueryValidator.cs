using FluentValidation;
using ShipNotes.Application.Common;
using ShipNotes.Domain.Versioning;

namespace ShipNotes.Application.PublicApi;

public sealed class PublicReleaseNotesRangeQueryValidator : AbstractValidator<PublicReleaseNotesRangeQuery>
{
    public PublicReleaseNotesRangeQueryValidator()
    {
        RuleFor(q => q.From).NotEmpty().DependentRules(() => RuleFor(q => q.From!).MustBeSemanticVersion());
        RuleFor(q => q.To).NotEmpty().DependentRules(() => RuleFor(q => q.To!).MustBeSemanticVersion());

        RuleFor(q => q)
            .Must(q => SemanticVersion.Parse(q.From!) <= SemanticVersion.Parse(q.To!))
            .When(q => SemanticVersion.TryParse(q.From, out _) && SemanticVersion.TryParse(q.To, out _))
            .WithName("From")
            .WithMessage("'From' must not be a later version than 'To'.");
    }
}
