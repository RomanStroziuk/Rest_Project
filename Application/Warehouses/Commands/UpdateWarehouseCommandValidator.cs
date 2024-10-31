using FluentValidation;

namespace Application.Warehouses.Commands;

public class UpdateWarehouseCommandValidator: AbstractValidator<UpdateWarehouseCommand>
{
    public UpdateWarehouseCommandValidator()
    {
        RuleFor(x => x.Location).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.TotalQuantity).GreaterThan(10).WithMessage("Quantity must be greater than 10");
    }
}