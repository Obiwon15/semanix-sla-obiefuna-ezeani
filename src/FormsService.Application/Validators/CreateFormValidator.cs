using FluentValidation;
using FormsService.Application.Commands;

namespace FormsService.Application.Validators;

public class CreateFormValidator : AbstractValidator<CreateFormCommand>
{
    public CreateFormValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("TenantId is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters");

        RuleFor(x => x.Sections)
            .NotEmpty()
            .WithMessage("At least one section is required");

        RuleForEach(x => x.Sections)
            .SetValidator(new SectionValidator());
    }
}