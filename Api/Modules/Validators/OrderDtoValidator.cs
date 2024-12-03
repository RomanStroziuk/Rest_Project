using FluentValidation;
using Api.Dtos.OrderDtos;

public class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order Id is required.");

        // Додаткові правила для інших властивостей OrderDto...
    }

    private bool BeAValidGuid(Guid id) => id != Guid.Empty;
}