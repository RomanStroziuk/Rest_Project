using Domain.Statuses;

namespace Tests.Data;

public static class StatusData
{
    public static Status MainStatus()
        => Status.New(StatusId.New(), "MainStatus");
}