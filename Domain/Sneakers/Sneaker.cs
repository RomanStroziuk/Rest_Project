using Domain.Brands;
using Domain.Сategories;

namespace Domain.Sneakers;

public class Sneaker
{

    public SneakerId Id { get; }
    
    public string Model  { get; private set; }
    
    public int Size { get; private set; }
    
    public int Price { get; private set; }
    
    

}