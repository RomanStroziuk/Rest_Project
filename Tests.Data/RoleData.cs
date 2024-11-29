using Domain.Roles;

namespace Tests.Data;

public static class RoleData
{
    public static Role AdminRole()
        => Role.New(RoleId.New(), "Admin");
    
    
    public static Role UserRole()
        => Role.New(RoleId.New(), "User");
    
    
    public static Role MainRole()
        => Role.New(RoleId.New(), "MainRole");
}