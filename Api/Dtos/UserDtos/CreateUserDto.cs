using Domain.Users;

namespace Api.Dtos.UserDtos;

public record CreateUserDto
(
    string FirstName,
    string LastName,
    string Email,
    string Password)
{
    public static CreateUserDto FromUser(User user)
        => new(
            FirstName: user.FirstName,
            LastName  : user.LastName,
            Email: user.Email,
            Password: user.Password);
}