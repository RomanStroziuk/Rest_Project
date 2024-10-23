using Domain.Brands;
using Domain.Sneakers;
using Domain.Сategories;

namespace Api.Dtos;

public record SneakerDto(
    Guid? Id,
    string model,
    int size,
    int price,
    Guid CategoryId,
    CategoryDto? Category,
    Guid BrandId, 
    BrandDto? Brand) 

{
    public static SneakerDto FromDomainModel(Sneaker sneaker)
        => new(
            Id: sneaker.Id.Value,
            model: sneaker.Model,
            size: sneaker.Size,
            price: sneaker.Price,
            CategoryId: sneaker.CategoryId.Value,
            Category: sneaker.Category == null ? null : CategoryDto.FromDomainModel(sneaker.Category),
            BrandId: sneaker.BrandId.Value,
            Brand: sneaker.Brand == null ? null : BrandDto.FromDomainModel(sneaker.Brand));
}