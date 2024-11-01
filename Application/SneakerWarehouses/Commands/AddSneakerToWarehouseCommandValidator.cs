using FluentValidation;

namespace Application.SneakerWarehouses.Commands;

public class AddSneakerToWarehouseCommandValidator : AbstractValidator<AddSneakerToWarehouseCommand>
{
    public AddSneakerToWarehouseCommandValidator()
    {
        RuleFor(x => x.SneakerId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}