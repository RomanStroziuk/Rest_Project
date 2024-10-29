using Domain.Roles;

namespace Domain.Users;

public class User
{
    public UserId Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public Role? Role { get; private set; }
    public RoleId RoleId { get; }

    private User(UserId id, string firstName, string lastName, string email, string password, RoleId roleId)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        RoleId = roleId;
    }
    
    public static User  New(UserId id, string firstName, string lastName, string email, string password, RoleId roleId)
    => new(id, firstName, lastName, email, password, roleId);

    public void UpdateDetails(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }
}