using Domain.Users;

namespace Api.Dtos.UserDtos;

public record UserUpdateInitialsDto(
    Guid? Id,
    string FirstName,
    string LastName)

{
    public static UserUpdateInitialsDto FromDomainModel(User user)
        => new(
            Id: user.Id.Value,
            FirstName: user.FirstName,
            LastName: user.LastName);
}