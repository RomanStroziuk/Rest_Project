using Domain.Brands;

namespace Tests.Data;

public static class BrandsData
{
    public static Brand MainBrand()
        => Brand.New(BrandId.New(), "TestBrand");
}