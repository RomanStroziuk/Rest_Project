using Api.Dtos;
using Api.Dtos.BrandDtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class BrandDtoValidator : AbstractValidator<BrandDto>
{
    public BrandDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
}