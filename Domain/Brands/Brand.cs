using Domain.Sneakers;

namespace Domain.Brands;

public class Brand
{
    public BrandId Id { get; }
    public string Name { get; private set; }


    private Brand(BrandId id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public static Brand New(BrandId id, string name)
        => new (id, name);
    
    public void UpdateDetails(string name) => Name = name;
}