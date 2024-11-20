using Domain.Roles;

namespace Api.Dtos.RoleDtos;

public record RoleDto(Guid? Id, string Title)
{
    public static RoleDto FromDomainModel(Role role)
        => new(role.Id.Value, role.Title);
}