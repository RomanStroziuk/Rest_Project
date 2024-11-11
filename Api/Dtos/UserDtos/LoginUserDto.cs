using Domain.Users;

namespace Api.Dtos.UserDtos;

public record LoginUserDto(string email, string password)
{
    public static LoginUserDto FromModelDomain(User user)
        => new(email: user.Email, password: user.Password);
}