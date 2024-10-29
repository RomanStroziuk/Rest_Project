using FluentValidation;

namespace Application.Brands.Commands;

public class DeleteBrandCommandValidator: AbstractValidator<DeleteBrandCommand>
{
    public DeleteBrandCommandValidator()
    {
        RuleFor(x => x.BrandId).NotEmpty();
    }
}