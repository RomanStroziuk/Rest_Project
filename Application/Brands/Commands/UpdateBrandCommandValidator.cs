using FluentValidation;

namespace Application.Brands.Commands;

public class UpdateFacultyCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    public UpdateFacultyCommandValidator()
    {
        RuleFor(x => x.BrandId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
    }
}