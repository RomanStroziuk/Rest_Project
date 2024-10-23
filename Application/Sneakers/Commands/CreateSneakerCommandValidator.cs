using FluentValidation;

namespace Application.Sneakers.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateSneakerCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(m => m.Model).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.BrandId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();

    }
}