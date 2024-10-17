
namespace Domain.Sneakers;

public record SneakerId(Guid Value)
{
    
    public static SneakerId New() => new(Guid.NewGuid());
    public static SneakerId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
    
    
}