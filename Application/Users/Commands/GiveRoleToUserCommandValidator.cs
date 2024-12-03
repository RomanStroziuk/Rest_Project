using FluentValidation;

namespace Application.Users.Commands;

public class GiveRoleToUserCommandValidator : AbstractValidator<GiveRoleToUserCommand>
{
    public GiveRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}