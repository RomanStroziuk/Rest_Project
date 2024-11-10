using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserFirstAndLastNameCommandValidator : AbstractValidator<UpdateUserFirstAndLastNameCommand>
{
    public UpdateUserFirstAndLastNameCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x=>x.FirstName).NotEmpty().MinimumLength(3).MaximumLength(30);
        RuleFor(x =>x.LastName ).NotEmpty().MinimumLength(3).MaximumLength(30);
    }
}