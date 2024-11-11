﻿using FluentValidation;

namespace Application.Users.Commands;

public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(x=>x.UserId).NotEmpty();
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number");
    }
}