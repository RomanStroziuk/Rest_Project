using FluentValidation;
using Api.Dtos.SneakerWarehouseDtos;

public class SneakerWarehouseDtoValidator : AbstractValidator<SneakerWarehouseDto>
{
    public SneakerWarehouseDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("SneakerWarehouse Id is required.");

        // Додаткові правила для інших властивостей SneakerWarehouseDto...
    }

    private bool BeAValidGuid(Guid id) => id != Guid.Empty;
}