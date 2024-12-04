using FluentValidation;
using Api.Dtos.OrderDtos;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.OrderDate)
            .NotEmpty().WithMessage("Order date is required.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Order date cannot be in the future.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.StatusId)
            .NotEmpty().WithMessage("Status ID is required.");
    }
}

 