namespace Domain.Statuses;

public record StatusId(Guid Value)
{
    public static StatusId New() => new(Guid.NewGuid());
    public static StatusId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}