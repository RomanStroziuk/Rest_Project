using Domain.Users;
using Domain.Roles;

namespace Tests.Data;

public static class UsersData
{
    public const string passwordAdmin = "password";

    public static User AdminUser(RoleId roleId)
        => User.New(UserId.New(), "Ali baba", "Baba Ali", "admin@gmail.com", passwordAdmin, roleId);
    
    public static User JustUser(RoleId roleId)
        => User.New(UserId.New(), "Ali baba Two", "Baba Ali Two", "user2@gmail.com", "password2", roleId);
}