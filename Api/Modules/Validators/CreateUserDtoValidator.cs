using FluentValidation;
using Api.Dtos.UserDtos;

namespace Api.Modules.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required.");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required.");
            RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address.");
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }
}