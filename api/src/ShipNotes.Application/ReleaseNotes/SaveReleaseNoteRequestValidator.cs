using FluentValidation;
using ShipNotes.Application.Common;
using ShipNotes.Domain.ReleaseNotes;

namespace ShipNotes.Application.ReleaseNotes;

public sealed class SaveReleaseNoteRequestValidator : AbstractValidator<SaveReleaseNoteRequest>
{
    public SaveReleaseNoteRequestValidator()
    {
        RuleFor(r => r.Version)
            .NotEmpty()
            .MustBeSemanticVersion();

        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(ReleaseNoteConstraints.TitleMaxLength);

        RuleFor(r => r.SummaryHtml)
            .MaximumLength(ReleaseNoteConstraints.ContentMaxLength);

        RuleFor(r => r.Sections)
            .Must(sections => sections!.DistinctBy(s => s.Category).Count() == sections!.Count)
            .WithMessage("'{PropertyName}' can only have one section per category.")
            .When(r => r.Sections is not null);

        RuleForEach(r => r.Sections).ChildRules(section =>
        {
            section.RuleFor(s => s.Category).IsInEnum();
            section.RuleFor(s => s.ContentHtml)
                .NotNull()
                .MaximumLength(ReleaseNoteConstraints.ContentMaxLength);
        });
    }
}
