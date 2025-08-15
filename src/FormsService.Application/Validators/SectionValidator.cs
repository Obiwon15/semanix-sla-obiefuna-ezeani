using FluentValidation;
using FormsService.Application.DTOs;

namespace FormsService.Application.Validators;

public class SectionValidator : AbstractValidator<SectionDto>
{
    public SectionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Section name is required");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("Section order must be greater than 0");

        RuleFor(x => x.Fields)
            .NotEmpty()
            .WithMessage("Section must have at least one field");

        RuleForEach(x => x.Fields)
            .SetValidator(new FieldDefinitionValidator());

        RuleFor(x => x.Fields)
            .Must(HaveUniqueOrders)
            .WithMessage("Field orders within a section must be unique");
    }

    private static bool HaveUniqueOrders(List<FieldDefinitionDto> fields)
    {
        var orders = fields.Select(f => f.Order).ToList();
        return orders.Count == orders.Distinct().Count();
    }
}