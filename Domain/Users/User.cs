using System.Runtime.Versioning;
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
    public RoleId RoleId { get;  private set; }

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
    
    public void UpdatePassword(string password)
    {
        Password = password;
    } 
    public void UpdateEmail(string email)
    {
        Email = email;
    } 
    public void UpdateUserLastAndFirstName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    public void SetNewRole(RoleId roleId)
    {
        RoleId = roleId;
    }
}