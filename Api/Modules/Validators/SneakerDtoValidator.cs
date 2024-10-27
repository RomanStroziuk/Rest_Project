using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class SneakerDtoValidator : AbstractValidator<SneakerDto>
{
    public SneakerDtoValidator()
    {
        RuleFor(x => x.model).NotEmpty().MaximumLength(255).MinimumLength(3);
        
        RuleFor(x => x.size)
            .NotEmpty().WithMessage("Size is required.")
            .InclusiveBetween(35, 50).WithMessage("Size must be between 35 and 50.");

        RuleFor(x => x.price)
            .NotEmpty().WithMessage("Price is required.");

    }
}