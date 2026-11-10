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

        RuleFor(r => r.ContentHtml)
            .NotNull()
            .MaximumLength(ReleaseNoteConstraints.ContentMaxLength);
    }
}
