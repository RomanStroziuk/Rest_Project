using FluentValidation;
using Api.Dtos.OrderItemDtos;
using Api.Dtos.SneakerWarehouseDtos;
using Api.Dtos.OrderDtos;

public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.SneakerWarehouseId)
            .NotEmpty().WithMessage("SneakerWarehouseId is required.");

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");
        
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        
        RuleFor(x => x.TotalPrice)
            .GreaterThan(0).WithMessage("TotalPrice must be greater than 0.");

        // Optionally, validate nested DTOs (if required)
        RuleFor(x => x.SneakerWarehouse)
            .NotNull().WithMessage("SneakerWarehouse must not be null.")
            .SetValidator(new SneakerWarehouseDtoValidator()); 

        RuleFor(x => x.Order)
            .NotNull().WithMessage("Order must not be null.")
            .SetValidator(new OrderDtoValidator());
    }

    
}