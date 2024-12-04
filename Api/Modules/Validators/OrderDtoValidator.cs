using FluentValidation;
using Api.Dtos.OrderDtos;

public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order Id is required.");

    }

    private bool BeAValidGuid(Guid id) => id != Guid.Empty;
}