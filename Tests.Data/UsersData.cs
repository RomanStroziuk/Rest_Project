using Domain.Users;
using Domain.Roles;

namespace Tests.Data;

public static class UsersData
{
    public static User MainUser(RoleId roleId)
        => User.New(UserId.New(), "Ali baba", "Baba Ali", "user@gmail.com", "password", roleId);
    
    public static User SecondUse(RoleId roleId)
        => User.New(UserId.New(), "Ali baba Two", "Baba Ali Two", "user2@gmail.com", "password2", roleId);
    
    public static User ThirdUser(RoleId roleId)
        => User.New(UserId.New(), "Ali baba Three", "Baba Ali Tree", "user3@gmail.com", "password3", roleId);
    
}