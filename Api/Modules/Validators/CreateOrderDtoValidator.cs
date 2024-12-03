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
            .NotEmpty().WithMessage("User ID is required.")
            .Must(BeAValidGuid).WithMessage("User ID must be a valid GUID.");

        RuleFor(x => x.StatusId)
            .NotEmpty().WithMessage("Status ID is required.")
            .Must(BeAValidGuid).WithMessage("Status ID must be a valid GUID.");
    }

    private bool BeAValidGuid(Guid userId)
    {
        // This can be customized to further check if GUID exists in the database or is valid according to your rules.
        return userId != Guid.Empty;
    }
}