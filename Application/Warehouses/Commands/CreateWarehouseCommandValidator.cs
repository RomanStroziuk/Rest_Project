using Domain.Warehouses;
using FluentValidation;

namespace Application.Warehouses.Commands;

public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
{
    public CreateWarehouseCommandValidator()
    {
        RuleFor(x => x.Location).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.TotalQuantity).GreaterThan(10).WithMessage("Quantity must be greater than 10");
    }
}