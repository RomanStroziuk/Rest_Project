using FluentValidation;

namespace Application.SneakerWarehouses.Commands;

public class UpdateSneakerQuantityInWarehouseCommandValidator : AbstractValidator<UpdateSneakerQuantityInWarehouseCommand>
{
    public UpdateSneakerQuantityInWarehouseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}