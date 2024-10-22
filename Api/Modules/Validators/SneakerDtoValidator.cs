using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class SneakerDtoValidator : AbstractValidator<SneakerDto>
{
    public SneakerDtoValidator()
    {
        RuleFor(x => x.model).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
}