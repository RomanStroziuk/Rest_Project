using Domain.Сategories;

namespace Tests.Data;

public static class CategoriesData
{
    public static Category MainCategory()
        => Category.New(CategoryId.New(), "TestCategory");
}