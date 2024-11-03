using Domain.Statuses;

namespace Tests.Data;

public static class StatusData
{
    public static Status MainStatus => Status.New(StatusId.New(), "Main Test Status");
    
    // You can add more sample roles if needed
    public static Status AnotherStatus => Status.New(StatusId.New(), "Another Test Status");
}