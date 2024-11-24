using Domain.Roles;

namespace Tests.Data;

public static class RolesData
{
    public static Role MainRole => Role.New(RoleId.New(), "Main Test Role");
    
    public static Role AnotherRole => Role.New(RoleId.New(), "Another Test Role");
}