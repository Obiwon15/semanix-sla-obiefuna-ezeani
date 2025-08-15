using FluentValidation;
using FormsService.Application.DTOs;

namespace FormsService.Application.Validators;

public class FieldDefinitionValidator : AbstractValidator<FieldDefinitionDto>
{
    public FieldDefinitionValidator()
    {
        RuleFor(x => x.FieldId)
            .NotEmpty()
            .WithMessage("FieldId is required");

        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessage("Field label is required");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("Field order must be greater than 0");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid field type");
    }
}