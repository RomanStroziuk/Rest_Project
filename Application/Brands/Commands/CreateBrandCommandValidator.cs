using FluentValidation;

namespace Application.Brands.Commands;

public class CreateBrandCommandValidator :AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
    
}