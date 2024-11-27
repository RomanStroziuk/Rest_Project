using Domain.Сategories;

namespace Api.Dtos.CategoryDtos;

public record CategoryDto(Guid? Id, string Name)
{
    public static CategoryDto FromDomainModel(Category category)
        => new(category.Id.Value, category.Name);
}