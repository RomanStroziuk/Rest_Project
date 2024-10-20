using Domain.Brands;

namespace Api.Dtos;

public record BrandDto(Guid? Id, string Name)
{
    public static BrandDto FromDomainModel(Brand brand)
        => new(brand.Id.Value, brand.Name);
}