using FluentValidation;
using ShipNotes.Application.Common;

namespace ShipNotes.Application.PublicApi;

public sealed class PublicReleaseNotesQueryValidator : AbstractValidator<PublicReleaseNotesQuery>
{
    public PublicReleaseNotesQueryValidator()
    {
        RuleFor(q => q.Limit).InclusiveBetween(1, PublicReleaseNotesQuery.MaxLimit);

        When(q => q.After is not null, () => RuleFor(q => q.After!).MustBeSemanticVersion());
        When(q => q.UpTo is not null, () => RuleFor(q => q.UpTo!).MustBeSemanticVersion());
    }
}
