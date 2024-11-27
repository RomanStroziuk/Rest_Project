using FluentValidation;

namespace Application.OrderItems.Commands;

public class UpdateOrderItemCommandValidator : AbstractValidator<UpdateOrderItemCommand>
{
    public UpdateOrderItemCommandValidator()
    {
        RuleFor(x => x.Quantity).NotEmpty()
            .LessThanOrEqualTo(5).WithMessage("Maximum 5 sneakers can be ordered at once");
        RuleFor(x => x.Quantity).NotEmpty();
    }
}