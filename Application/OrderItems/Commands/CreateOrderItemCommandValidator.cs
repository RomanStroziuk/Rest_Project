
using FluentValidation;

namespace Application.OrderItems.Commands;

public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
{
    public CreateOrderItemCommandValidator()
    {
        RuleFor(x => x.Quantity).NotEmpty()
            .LessThanOrEqualTo(5).WithMessage("Maximum 5 sneakers can be ordered at once");
    }
}