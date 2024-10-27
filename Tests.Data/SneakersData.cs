using Domain.Brands;
using Domain.Сategories;
using Domain.Sneakers;

namespace Tests.Data;

public static class UsersData
{
    public static Sneaker MainSneaker(CategoryId categoryId, BrandId brandId)
        => Sneaker.New(SneakerId.New(), "Model Name One", 35, 2900, brandId, categoryId);
    
    public static Sneaker SecondSneaker(CategoryId categoryId, BrandId brandId)
        => Sneaker.New(SneakerId.New(), "Model Name Two", 39, 3900, brandId, categoryId);
    
    public static Sneaker ThirdSneaker(CategoryId categoryId, BrandId brandId)
        => Sneaker.New(SneakerId.New(), "Model Name Three", 44, 2600, brandId, categoryId);
    
}