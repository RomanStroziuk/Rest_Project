using FluentValidation;

namespace Application.Sneakers.Commands;

public class DeleteSneakerCommandValidator : AbstractValidator<DeleteSneakerCommand>
{
    public DeleteSneakerCommandValidator()
    {
        RuleFor(x => x.SneakerId).NotEmpty();
    }
}