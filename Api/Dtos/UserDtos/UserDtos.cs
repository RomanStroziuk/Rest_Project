using Api.Dtos.RoleDtos;
using Domain.Users;

namespace Api.Dtos.UserDtos;

public record UserDto(
    Guid? Id,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    Guid RoleId,
    RoleDto? Role)

{
    public static UserDto FromDomainModel(User user)
        => new(
            Id: user.Id.Value,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            Password: user.Password,
            RoleId: user.RoleId.Value,
            Role: user.Role == null ? null : RoleDto.FromDomainModel(user.Role));

}