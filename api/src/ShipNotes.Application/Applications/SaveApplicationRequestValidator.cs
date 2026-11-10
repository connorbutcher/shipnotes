using FluentValidation;
using ShipNotes.Application.Common;
using ShipNotes.Domain.Applications;

namespace ShipNotes.Application.Applications;

public sealed class SaveApplicationRequestValidator : AbstractValidator<SaveApplicationRequest>
{
    public SaveApplicationRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(ApplicationConstraints.NameMaxLength);

        RuleFor(r => r.IconUrl)
            .MaximumLength(ApplicationConstraints.IconUrlMaxLength)
            .MustBeAbsoluteHttpUrl();
    }
}
