namespace Domain.Statuses;

public class Status
{
    public StatusId Id { get; }
    public string Title { get; private set; }

    private Status(StatusId id, string title)
    {
        Id = id;
        Title = title;
    }

    public static Status New(StatusId id, string title)
        => new(id, title);

    public void ChangeTitle(string newTitle)
    {
        Title = newTitle;
    }
}